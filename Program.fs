// Learn more about F# at http://fsharp.org

open System
open System.Diagnostics
open Elmish.WPF
open Elmish
open System.Windows.Controls
open System.Windows.Threading
// this is the custom controls project with the xaml files
open WpfControls
open GenericTabs

// an f# algabraic type to represent the data
type Model = {
    ClickCount: int
    Message: string
    Name: string
}

// a type defining the messages that the application will handle
// there will be a message for each twoWay binding, ie that modifies the state
// a message for each button click
type MessageType =
    | ButtonClicked
    | Reset
    | ChangeToWinkle
    | SetName of string
    | AddTab

// set up defaults for the model
let init() = {
    ClickCount = 0
    Message = "Hello Elmish.WPF"
    Name = "Fred"
}

let addTab() = 
    let newTab = new TabItem()
    newTab.Header <- "Test"
    newTab.Name <- "tabNew"
    newTab.Content <- new UserControl1()
    let mainWin : Window1 = downcast System.Windows.Application.Current.MainWindow
    Trace.WriteLine mainWin.Name
    let newTabIndex = 
        match mainWin.tabMainGlobal with
        | null -> 0
        | _ -> mainWin.tabMainGlobal.Items.Add(newTab)

    mainWin.Dispatcher.BeginInvoke(DispatcherPriority.Normal, Action(fun () -> mainWin.tabMainGlobal.SelectedItem <- newTab)) |> ignore
    //let newTabIndex = mainWin.tabMainGlobal.Items.Add(new TabItem())
//    let mainWin : Window1 = System.Windows.Application.Current.MainWindow :?> Window1
    //mainWin.tabMainGlobal.Items.Add(newTab)
    let newModel ={
        ClickCount = 0
        Message = "Hello Elmish.WPF"
        Name = "Added the tab"
    }
    newModel


//this is like mediator pattern with a big switch 
//on all possible messages
// a two way binding will send a message, eg SetName with a parameter
// each match pattern will return a new model with modifications
let update (msg: MessageType) (model: Model) : Model =
    match msg with
        | ButtonClicked -> { model with ClickCount = model.ClickCount + 1; Name = model.Name + (model.ClickCount + 1).ToString() }
        | Reset -> init()
        | ChangeToWinkle -> { model with Name = model.Name + " Winkle"}
        | SetName name -> { model with Name = name } 
        | AddTab -> addTab()

          


// one off method, called at startup to set up bindings from  xaml to functions and types
//oneWay bindings
// twoWay bindings
// command bindings - flag a message that will be handled via the Update function
let bindings() =
    [
        //one-way bindings
        "ClickCount" |> Binding.oneWay (fun m -> m.ClickCount)
        "Message" |> Binding.oneWay (fun m -> m.Message)
        "Name" |> Binding.twoWay (
          (fun m -> m.Name),
          //(fun v m -> SetName(v))
          //this is shorthand for above, model is not required, can do either, value is passed implicitly
          (SetName))
        //commands
        "ClickCommand" |> Binding.cmd ButtonClicked
        "ResetCommand" |> Binding.cmd Reset
        "AddTabCommand" |> Binding.cmd AddTab
        //two ways to bind command, pass model with lambda or just the Message
        //"ChangeToWinkle" |> Binding.cmd ChangeToWinkle
        "ChangeToWinkle" |> Binding.cmd (fun m -> 
        Trace.WriteLine("Winkle called:")
        ChangeToWinkle)
    ]


[<EntryPoint; STAThread>]
let main argv =
    printfn "Hello World from F#! you many man"
    Program.mkSimpleWpf init update bindings 
    |> Program.withConsoleTrace
    |> Program.runWindowWithConfig{
      ElmConfig.Default with LogConsole = true; Measure = true
      }
      (Window1())
    0 // return an integer exit code
