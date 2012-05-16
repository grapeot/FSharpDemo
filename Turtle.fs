// Copy the script into F# interactive console to run

#r "System.Windows.Forms.dll";;
#r "System.Drawing.dll";;

open System
open System.Windows.Forms
open System.Drawing

let mutable p = new PointF(300.0f, 250.0f)
let mutable angle = 0.0
let mutable canvas = new Bitmap(1000, 700)
let mutable g = Graphics.FromImage canvas
let _Fwd pixel = 
    let pEnd = new PointF(p.X + float32(pixel * Math.Cos(angle)), p.Y + float32(pixel * Math.Sin(angle)))
    g.DrawLine(Pens.Black, p, pEnd)
    p <- pEnd
let _Left degree = 
    angle <- angle - degree * Math.PI / 180.0
let _Right degree = 
    angle <- angle + degree * Math.PI / 180.0
let showCanvas img = 
    let mutable form = new Form(WindowState = FormWindowState.Maximized)
    let mutable pictureBox = new PictureBox(Image = img, Dock = DockStyle.Fill)
    form.Controls.Add pictureBox
    form.ShowDialog()

type Command = Fwd | Left | Right
let Fwd x = (Fwd, x)
let Left x = (Left, x)
let Right x = (Right, x)

let kochChange drawing acc = [
    for cmd in drawing do
        match cmd with
        | (Fwd, n) -> 
            let x = n / 3.0
            yield Fwd x
            yield Left 60.
            yield Fwd x
            yield Right 120.
            yield Fwd x
            yield Left 60.
            yield Fwd x
        | c -> yield c ]
let koch1 = [ Fwd 400.; Right 120.; Fwd 400.; Right 120.; Fwd 400. ]

[1..4] |> List.fold kochChange koch1 |> List.iter(fun x -> 
    match x with
    | (Fwd, n) -> _Fwd(n)
    | (Left, n) -> _Left(n)
    | (Right, n) -> _Right(n))

g.Dispose()
showCanvas canvas;;