// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open QueuingSystem
open System



[<EntryPoint>]
let main argv = 

    let task()=
         System.Threading.Thread.Sleep(5000) |> ignore
         printfn "%s" (System.Threading.Thread.CurrentThread.ManagedThreadId.ToString())
         
    let taskerror()=
        System.Threading.Thread.Sleep(5000) |> ignore
        failwith "AHA, this is an error"
    
    let token = new System.Threading.CancellationTokenSource()
    let consumer = new ConsumerSystem(4,token)

    let msg1 = System.Threading.Tasks.Task<JOBRESULT>.Factory
                    .StartNew(fun () -> consumer.AddJob(4,new System.Action(task)))

    let error =  System.Threading.Tasks.Task<JOBRESULT>.Factory
                    .StartNew(fun () -> consumer.AddJob(4,new System.Action(taskerror)))

    let result1 = msg1.Result
    let resutl2 = error.Result;
    
    consumer.Stop()
    //printfn "%A" r
    0 // return an integer exit code
