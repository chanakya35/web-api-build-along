namespace Example

open System
open System.Configuration
open System.ServiceProcess
open System.Configuration.Install
open System.ComponentModel

(* // from phase 1 only
module Main =
    let endPoint = new EndPoint()
    endPoint.StartEndPoint()
    Console.ReadLine() |> ignore
*)

module Constants =
    let serviceName = "MyService"

type Svc() =
    inherit ServiceBase(ServiceName = Constants.serviceName)

    let endPoint = new EndPoint()

    override this.OnStart(args) =
        endPoint.StartEndPoint()

    override this.OnStop() =
        endPoint.StopEndPoint()
        Async.CancelDefaultToken()

[<RunInstaller(true)>]
type SvcHost() =
    inherit Installer()

    let serviceInstaller =
        new ServiceInstaller
            ( DisplayName = Constants.serviceName,
              ServiceName = Constants.serviceName,
              StartType = ServiceStartMode.Manual)
    do
        new ServiceProcessInstaller(Account = ServiceAccount.LocalSystem)
        |> base.Installers.Add |> ignore

        serviceInstaller
        |> base.Installers.Add |> ignore

    let matchService (this:SvcHost) =
        match this.Context.Parameters.ContainsKey("ServiceName") with
        | true ->
            serviceInstaller.DisplayName <- this.Context.Parameters.["ServiceName"]
            serviceInstaller.ServiceName <- this.Context.Parameters.["ServiceName"]
        | false -> ()

    override this.Install(state) =
        matchService this
        base.Install(state)

    override this.Uninstall(state) =
        matchService this
        base.Uninstall(state)

module Main =
    ServiceBase.Run [| new Svc() :> ServiceBase |]

// OR, to create service directly use service control (as admin):
// sc create "MyService" binPath= "C:\<path>\WebApiBuildAlong\bin\Debug\WebApiBuildAlong.exe"
