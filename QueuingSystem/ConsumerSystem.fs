namespace QueuingSystem
open System.Collections.Generic
open System.Data
open System.Threading.Tasks

type JOBRESULT=
    |SUCCESS
    |FAILURE of System.Exception
type JOBMESSAGE= {NSHARE:int;CHANNEL:AsyncReplyChannel<JOBRESULT>;TASK:System.Action}
type MODELMESSAGE =
| KILL
| JOB of JOBMESSAGE



type ConsumerSystem(nworkers:int, token:System.Threading.CancellationTokenSource)=
            
    let queue = new System.Collections.Concurrent.ConcurrentQueue<JOBMESSAGE>()
    
    let consumer(token:System.Threading.CancellationToken) =
        while not(token.IsCancellationRequested) do
            match queue.TryDequeue() with
            |false,_ -> System.Threading.Thread.Sleep(500)
            |true,job -> 
                         try
                         job.TASK.Invoke()
                         job.CHANNEL.Reply(SUCCESS)
                         with
                         |ex  -> job.CHANNEL.Reply (FAILURE ex)
    let workers=        
                    [for i in 0 .. nworkers-1 do yield Task.Factory.StartNew(new System.Action(fun () -> consumer(token.Token)))]
                    
    
    
    let JOBQueuingSystem  =
        MailboxProcessor.Start(fun inbox ->
            let rec loop n =
                async { let! msg = inbox.Receive()
                        match msg with
                        | KILL -> return ()
                        | JOB(job) ->
                            queue.Enqueue job
                            //replyChannel.Reply(n)
                            return! loop(n) }
            loop 0)

   
    
    member  self.AddJob(nshare:int,task:System.Action)=
        JOBQueuingSystem.PostAndReply(fun replyChannel -> MODELMESSAGE.JOB {NSHARE=nshare;CHANNEL=replyChannel;TASK=task})

    member self.Stop()=
        JOBQueuingSystem.Post KILL
        token.Cancel()

