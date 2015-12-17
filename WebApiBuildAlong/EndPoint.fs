namespace Example

open System
open Owin
open System.Web.Http
open System.Configuration
open Microsoft.Owin.Hosting

// Startup type is a nice way to simplify endpoint config
type Startup() =
    member this.Configuration(app: IAppBuilder) =
        try
            let config = new HttpConfiguration()
            config.MapHttpAttributeRoutes()
            app.UseWebApi(config) |> ignore
        with ex ->
            printfn "%A" ex

type EndPointState =
    | Running of IDisposable
    | NotRunning 

type AgentSignal =
    | Start
    | Stop

type EndPoint() =
    let agent = MailboxProcessor.Start(fun inbox ->
        let rec loop state = async {
            let! msg = inbox.Receive()
            match msg, state with
            | Start, NotRunning ->
                let baseAddress = ConfigurationManager.AppSettings.["baseAddress"]
                let server = WebApp.Start<Startup>(baseAddress)
                printfn "Listening at %s" baseAddress
                return! loop (Running(server))
            | Stop, Running(server) ->
                server.Dispose()
                return! loop NotRunning
            | Start, Running(_)
            | Stop, NotRunning ->
                return! loop state                           
        }
        loop NotRunning)

    member this.StartEndPoint() =
        agent.Post(Start)

    member this.StopEndPoint() =
        agent.Post(Stop)
