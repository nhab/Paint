
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Paint
{
  public class ClipboardTool : RectangleToolBase
  {
    private ClipboardAction action;
    private Rectangle prevRect;
    private Rectangle rect;
    private Pen delPen;
    private Pen pen;
    private Point curPoint;

    public ClipboardTool(ToolArgs args, ClipboardAction action)
      : base(args) {
      this.action = action;
      args.pictureBox.MouseClick += new MouseEventHandler(OnMouseClick);
    }

    private void OnMouseClick(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Right) {
        if (Clipboard.ContainsImage()) {
          PasteImage(curPoint);
          args.pictureBox.Invalidate();
        }
      }
    }

    protected override void OnMouseDown(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        drawing = true;
        sPoint = e.Location;
        g = args.pictureBox.CreateGraphics();
        pen = Pens.Black;
        delPen = new Pen(new TextureBrush(args.bitmap), 1);
      }
    }


    private static String HexConverter(System.Drawing.Color c)
    {
        return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
    }

    protected override void OnMouseMove(object sender, MouseEventArgs e) {
      curPoint = e.Location;
      if (drawing) {
        // delete old
        g.DrawRectangle(delPen, prevRect);
        // draw the new rectangle
        rect = GetRectangleFromPoints(sPoint, curPoint);
        g.DrawRectangle(pen, rect);

        prevRect = rect;

        Color pixColor =new Color();
        try
        {
          Point loc = e.Location;

          pixColor = args.bitmap.GetPixel(sPoint.X, sPoint.Y);
         
        }
        catch (Exception ex)
        { }
        string sHexColor = HexConverter(pixColor);
        ///////////////////
            ShowPointInStatusBar(sPoint, sHexColor);
      } else {
        ShowPointInStatusBar(e.Location);
      }
    }

    protected override void OnMouseUp(object sender, MouseEventArgs e) {
      drawing = false;
      if (e.Button == MouseButtons.Left) {
        if ((action == ClipboardAction.Copy) || (action == ClipboardAction.Cut)) {
          // copy rectangle
          Bitmap copiedBmp = args.bitmap.Clone(rect, args.bitmap.PixelFormat);
          Clipboard.SetImage(copiedBmp);
          if (action == ClipboardAction.Cut) {
            // delete copied rectangle
            Graphics g = Graphics.FromImage(args.bitmap);
            g.FillRectangle(new SolidBrush(args.settings.SecondaryColor), rect);
          }
        } else if (action == ClipboardAction.Paste) {
          if (Clipboard.ContainsImage())
            PasteImage(rect);
        }
        args.pictureBox.Invalidate();
      }
    }

    private void PasteImage(Rectangle rect) {
      Graphics gc = Graphics.FromImage(args.bitmap);
      gc.DrawImage(Clipboard.GetImage(), rect);
    }

    private void PasteImage(Point p) {
      Graphics gc = Graphics.FromImage(args.bitmap);
      gc.DrawImage(Clipboard.GetImage(), p);
    }

    public override void UnloadTool() {
      base.UnloadTool();
      args.pictureBox.MouseClick -= new MouseEventHandler(OnMouseClick);
    }
  }
}
