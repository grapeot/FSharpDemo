// Copy the script into F# interactive console to run

#r "System.Windows.Forms.dll";;
#r "System.Drawing.dll";;

open System
open System.Numerics
open System.Drawing
open System.Windows.Forms

let maxIter = 255 * 2
let modSquared (c:Complex) = c.Real * c.Real + c.Imaginary * c.Imaginary
type MandelbrotResult = 
    | DidNotEscape
    | Escaped of int

let mandelbrot c =
    let rec _mandelbrot z i =
        if (modSquared z >= 4.0) then Escaped i
        elif i = maxIter then DidNotEscape
        else _mandelbrot (z * z + c) (i + 1)
    _mandelbrot c 0

let yBegin = -0.8
let yEnd = 0.8
let xBegin = -1.6
let xEnd = 0.3
let resolution = 0.0025
let convertX x = int((x - xBegin) / resolution)
let convertY y = int((y - yBegin) / resolution)

let sizeX = convertX xEnd + 1
let sizeY = convertY yEnd + 1
let mutable img = new Bitmap(sizeX, sizeY)
let mutable g = Graphics.FromImage img
g.Clear(Color.Black)
g.Dispose()
let mutable result = Array.create (sizeX * sizeY) (xBegin, yBegin, 0)

[ for y in [yBegin..resolution..yEnd] -> async{
    for x in [xBegin..resolution..xEnd] do
        result.[convertY y * sizeX + convertX x] <-
            match mandelbrot (Complex(x, y)) with
            | DidNotEscape -> (x, y, 0)
            | Escaped i -> (x, y, i / 2) } ]
|> Async.Parallel |> Async.RunSynchronously |> ignore
printfn "Begin filling pixels..."
result |> Array.iter (fun (x, y , c) ->
    img.SetPixel(convertX x, convertY y, Color.FromArgb(255, c, c, c)))

let mutable form = new Form(WindowState = FormWindowState.Maximized)
let mutable pictureBox = new PictureBox(Image = img, Dock = DockStyle.Fill)
form.Controls.Add pictureBox
form.ShowDialog()
img.Dispose();;