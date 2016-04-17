using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace DotNetOpenSource.Controls
{
	/// <summary>
	/// Summary description for ImageMap.
	/// </summary>
	[ToolboxBitmap(typeof(ImageMap))]
	public class ImageMap : System.Windows.Forms.UserControl
	{
		private System.Drawing.Drawing2D.GraphicsPath _pathData;
		private int _activeIndex = -1;
		private ArrayList _pathsArray;
		private ToolTip _toolTip;
		private Graphics _graphics;

		private System.Windows.Forms.PictureBox pictureBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public delegate void RegionClickDelegate(int index, string key);
		[Category("Action")]
		public event RegionClickDelegate RegionClick;

		public ImageMap()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			this._pathsArray = new ArrayList();
			this._pathData = new System.Drawing.Drawing2D.GraphicsPath();
			this._pathData.FillMode = System.Drawing.Drawing2D.FillMode.Winding;

			this.components = new Container();
			this._toolTip = new ToolTip(this.components);
			this._toolTip.AutoPopDelay = 5000;
			this._toolTip.InitialDelay = 1000;
			this._toolTip.ReshowDelay = 500;

			this._graphics = Graphics.FromHwnd(this.pictureBox.Handle);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// pictureBox
			// 
			this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(150, 150);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			this.pictureBox.Click += new System.EventHandler(this.pictureBox_Click);
			this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
			this.pictureBox.MouseLeave += new System.EventHandler(this.pictureBox_MouseLeave);
			// 
			// ImageMap
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pictureBox});
			this.Name = "ImageMap";
			this.ResumeLayout(false);

		}
		#endregion

		[Category("Appearance")]
		public Image Image
		{
			get
			{
				return this.pictureBox.Image;
			}
			set
			{
				this.pictureBox.Image = value;
			}
		}

		public int AddElipse(string key, Point center, int radius)
		{
			return this.AddElipse(key, center.X, center.Y, radius);
		}

		public int AddElipse(string key, int x, int y, int radius)
		{
			if(this._pathsArray.Count > 0)
				this._pathData.SetMarkers();
			this._pathData.AddEllipse(x - radius, y - radius, radius * 2, radius * 2);
			return this._pathsArray.Add(key);
		}

		public int AddRectangle(string key, int x1, int y1, int x2, int y2)
		{
			return this.AddRectangle(key, new Rectangle(x1, y1, (x2 - x1), (y2 - y1)));
		}

		public int AddRectangle(string key, Rectangle rectangle)
		{
			if(this._pathsArray.Count > 0)
				this._pathData.SetMarkers();
			this._pathData.AddRectangle(rectangle);
			return this._pathsArray.Add(key);
		}

		public int AddPolygon(string key, Point[] points)
		{
			if(this._pathsArray.Count > 0)
				this._pathData.SetMarkers();
			this._pathData.AddPolygon(points);
			return this._pathsArray.Add(key);
		}

		private void pictureBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int newIndex = this.getActiveIndexAtPoint(new Point(e.X, e.Y));
			if(newIndex > -1)
			{
				pictureBox.Cursor = Cursors.Hand;
				if(this._activeIndex != newIndex)
					this._toolTip.SetToolTip(this.pictureBox, this._pathsArray[newIndex].ToString());
			}
			else
			{
				pictureBox.Cursor = Cursors.Default;
				this._toolTip.RemoveAll();
			}
			this._activeIndex = newIndex;
		}

		private void pictureBox_MouseLeave(object sender, System.EventArgs e)
		{
			this._activeIndex = -1;
			this.Cursor = Cursors.Default;
		}

		private void pictureBox_Click(object sender, System.EventArgs e)
		{
			Point p = this.PointToClient(Cursor.Position);
			if(this._activeIndex == -1)
                this.getActiveIndexAtPoint(p);
			if(this._activeIndex > -1 && this.RegionClick != null)
				this.RegionClick(this._activeIndex, this._pathsArray[this._activeIndex].ToString());
		}

		private int getActiveIndexAtPoint(Point point)
		{
			System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
			System.Drawing.Drawing2D.GraphicsPathIterator iterator = new System.Drawing.Drawing2D.GraphicsPathIterator(_pathData);
			iterator.Rewind();
			for(int current=0; current < iterator.SubpathCount; current++)
			{
				iterator.NextMarker(path);
				if(path.IsVisible(point, this._graphics))
					return current;
			}
			return -1;
		}

		[Browsable(false)]
		public override Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
			set
			{
				base.BackgroundImage = value;
			}
		}
	}
}
