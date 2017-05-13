using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace Game
{
	class Tetris : Control
	{
		enum Tile { Empty, Red, Green, Blue, Yellow, Orange, Purple, Cyan };
		enum ColorSet { Solid, Emboss, Glass };

		class Map
		{
			public Size Size
			{
				get { return new Size(Width, Height); }
				set
				{
					Width = value.Width;
					Height = value.Height;
					_map = new Tile[Width, Height];
				}
			}

			public ColorSet ColorSet { get; set; }

			public int Width { get; private set; }
			public int Height { get; private set; }
			public int SquareWidth { get; set; }

			Tile[,] _map = new Tile[0, 0];

			public Map()
			{
				SquareWidth = 16;
			}

			public Map(Size s)
				: this()
			{
				Size = s;
			}

			public Map(int width, int height)
				: this()
			{
				Size = new Size(width, height);
			}

			public bool CheckCollision(Piece p)
			{
				for (int i = 0; i < 4; ++i)
					for (int j = 0; j < 4; ++j)
					{
						int x = p.Location.X + i;
						int y = p.Location.Y + j;
						if ((x < 0 && p.Block[i, j] != Tile.Empty) ||
							x >= Width && p.Block[i, j] != Tile.Empty)
							return true;

						if (y >= Height && p.Block[i, j] != Tile.Empty)
							return true;

						if (p.Block[i, j] != Tile.Empty && _map[x, y] != Tile.Empty)
							return true;
					}

				return false;
			}

			void DrawBlock(Graphics gfx, Tile color, float x, float y)
			{
				Image brick = null;

				switch (color)
				{
					case Tile.Empty:
						return;
					case Tile.Blue:
						switch (ColorSet)
						{
							case ColorSet.Solid:
								brick = Properties.Resources._1;
								break;
							case ColorSet.Emboss:
								brick = Properties.Resources._11;
								break;
							case ColorSet.Glass:
								brick = Properties.Resources._12;
								break;
						}
						break;
					case Tile.Cyan:
						switch (ColorSet)
						{
							case ColorSet.Solid:
								brick = Properties.Resources._2;
								break;
							case ColorSet.Emboss:
								brick = Properties.Resources._21;
								break;
							case ColorSet.Glass:
								brick = Properties.Resources._22;
								break;
						}
						break;
					case Tile.Green:
						switch (ColorSet)
						{
							case ColorSet.Solid:
								brick = Properties.Resources._3;
								break;
							case ColorSet.Emboss:
								brick = Properties.Resources._31;
								break;
							case ColorSet.Glass:
								brick = Properties.Resources._32;
								break;
						}
						break;
					case Tile.Orange:
						switch (ColorSet)
						{
							case ColorSet.Solid:
								brick = Properties.Resources._4;
								break;
							case ColorSet.Emboss:
								brick = Properties.Resources._41;
								break;
							case ColorSet.Glass:
								brick = Properties.Resources._42;
								break;
						}
						break;
					case Tile.Purple:
						switch (ColorSet)
						{
							case ColorSet.Solid:
								brick = Properties.Resources._5;
								break;
							case ColorSet.Emboss:
								brick = Properties.Resources._51;
								break;
							case ColorSet.Glass:
								brick = Properties.Resources._52;
								break;
						}
						break;
					case Tile.Red:
						switch (ColorSet)
						{
							case ColorSet.Solid:
								brick = Properties.Resources._6;
								break;
							case ColorSet.Emboss:
								brick = Properties.Resources._61;
								break;
							case ColorSet.Glass:
								brick = Properties.Resources._62;
								break;
						}
						break;
					case Tile.Yellow:
						switch (ColorSet)
						{
							case ColorSet.Solid:
								brick = Properties.Resources._7;
								break;
							case ColorSet.Emboss:
								brick = Properties.Resources._71;
								break;
							case ColorSet.Glass:
								brick = Properties.Resources._72;
								break;
						}
						break;
				}

				//gfx.DrawImage(brush, x, y, SquareWidth, SquareWidth);
				gfx.DrawImage(brick, x, y, SquareWidth, SquareWidth);
			}

			public void DrawPiece(Graphics g, Piece p)
			{
				for (int i = 0; i < 4; ++i)
					for (int j = 0; j < 4; ++j)
						DrawBlock(g, p.Block[i, j], (p.Location.X + i) * SquareWidth, (p.Location.Y + j) * SquareWidth);
			}

			public void Draw(Graphics g)
			{
				g.FillRectangle(Brushes.Black, 0, 0, Width * SquareWidth, Height * SquareWidth);
				for (int i = 0; i < Width; ++i)
					for (int j = 0; j < Height; ++j)
						DrawBlock(g, _map[i, j], i * SquareWidth, j * SquareWidth);
			}

			public void DrawNext(Graphics g, Piece p, Color backColor)
			{
				g.FillRectangle(new SolidBrush(backColor), (Width + 2) * SquareWidth, SquareWidth, 4 * SquareWidth, 4 * SquareWidth);
				for (int i = 0; i < 4; ++i)
					for (int j = 0; j < 4; ++j)
						DrawBlock(g, p.Block[i, j], (Width + 2 + i) * SquareWidth, (1 + j) * SquareWidth);
			}

			public void DrawRandom(Graphics g)
			{
				for (int i = 0; i < 5; ++i)
				{
					var s = new Piece(Piece.Rand.Next() % 10, Piece.Rand.Next() % 30);
					Add(s);
				}

				Draw(g);
			}

			public void Add(Piece p)
			{
				for (int i = 0; i < 4; ++i)
					for (int j = 0; j < 4; ++j)
					{
						if (p.Block[i, j] == Tile.Empty)
							continue;

						int x = Math.Min(Width - 1, i + p.Location.X);
						int y = Math.Min(Height - 1, j + p.Location.Y);

						if (_map[x, y] == Tile.Empty && p.Block[i, j] != Tile.Empty)
							_map[x, y] = p.Block[i, j];
					}
			}

			public void Clear()
			{
				for (int i = 0; i < Width; ++i)
					for (int j = 0; j < Height; ++j)
						_map[i, j] = Tile.Empty;
			}

			public bool RemoveRows()
			{
				bool redraw = false;
				for (int j = 1; j < Height; ++j)
				{
					bool fullRow = true;
					for (int i = 0; i < Width; ++i)
						if (_map[i, j] == Tile.Empty)
						{
							fullRow = false;
							break;
						}

					if (fullRow)
					{
						for (int k = j; k > 1; --k)
							for (int i = 0; i < Width; ++i)
								_map[i, k] = _map[i, k - 1];
						redraw = true;
					}
				}

				return redraw;
			}
		}

		Map _map = new Map(10, 30);
		Piece _currPiece;
		Piece _nextPiece;
		bool _isRunning;

		Timer _timer = new Timer();

		[Description("Size of the tile")]
		public Size MapSize
		{
			get { return new Size(_map.Width, _map.Height); }
			set { _map = new Map(value); }
		}

		[Description("Width of a single square")]
		public int SquareWidth
		{
			get { return _map.SquareWidth; }
			set { _map.SquareWidth = value; }
		}

		public int Set
		{
			get { return (int)_map.ColorSet; }
			set { _map.ColorSet = (ColorSet)value; }
		}

		protected override bool IsInputKey(Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Right:
				case Keys.Left:
				case Keys.Up:
				case Keys.Down:
					return true;
			}
			return base.IsInputKey(keyData);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			int dX = 0;
			int dY = 0;
			bool rotate = false;
			Piece tmpPiece = null;
			switch (e.KeyCode)
			{
				case Keys.Left:
					dX = -1;
					break;
				case Keys.Right:
					dX = 1;
					break;
				case Keys.Up:
					rotate = true;
					break;
				case Keys.Down:
					dY = 1;
					break;
				case Keys.Space:
					if (!_isRunning)
						return;

					while (_currPiece != null && !_map.CheckCollision(tmpPiece = _currPiece.Move(0, 1)))
						_currPiece = tmpPiece;

					NextPiece();
					_map.RemoveRows();
					Invalidate();
					return;
			}

			tmpPiece = rotate ? _currPiece.Rotate() : _currPiece.Move(dX, dY);
			if (!_map.CheckCollision(tmpPiece))
				_currPiece = tmpPiece;

			Invalidate();
		}

		public bool IsRunning { get { return _isRunning; } }

		public Tetris()
		{
			_timer.Interval = 250;
			_timer.Tick += OnTick;
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
		}

		public void NewGame()
		{
			Reset();

			int x = ((_map.Width - 4) / 2);
			_currPiece = new Piece(x, 0);
			_nextPiece = new Piece(x, 0);
			_isRunning = true;
			Start();
		}

		public void Start()
		{
			_timer.Start();
		}

		public void Stop()
		{
			_timer.Stop();
		}

		public void Reset()
		{
			_timer.Stop();
			_map.Clear();
			_currPiece = null;
			_nextPiece = null;
			Invalidate();
		}

		class Piece
		{
			public static Random Rand = new Random(Environment.TickCount);

			public Tile[,] Block = new Tile[4, 4];
			public Point Location;

			public Piece()
			{
				int tile = Rand.Next(1, 7);
				switch ((Tile)tile)
				{
					case Tile.Red: // tower
						Block[1, 0] = Tile.Red;
						Block[1, 1] = Tile.Red;
						Block[1, 2] = Tile.Red;
						Block[1, 3] = Tile.Red;
						break;
					case Tile.Cyan: // box
						Block[1, 1] = Tile.Cyan;
						Block[2, 1] = Tile.Cyan;
						Block[2, 2] = Tile.Cyan;
						Block[2, 3] = Tile.Cyan;
						break;
					case Tile.Blue: // Square
						Block[1, 1] = Tile.Blue;
						Block[1, 2] = Tile.Blue;
						Block[2, 1] = Tile.Blue;
						Block[2, 2] = Tile.Blue;
						break;
					case Tile.Green: // RightLeaner
						Block[2, 1] = Tile.Green;
						Block[1, 1] = Tile.Green;
						Block[1, 2] = Tile.Green;
						Block[0, 2] = Tile.Green;
						break;
					case Tile.Purple: // RightKnight
						Block[2, 1] = Tile.Purple;
						Block[1, 1] = Tile.Purple;
						Block[1, 2] = Tile.Purple;
						Block[1, 3] = Tile.Purple;
						break;
					case Tile.Orange: // Pyramid
						Block[1, 1] = Tile.Orange;
						Block[0, 2] = Tile.Orange;
						Block[1, 2] = Tile.Orange;
						Block[2, 2] = Tile.Orange;
						break;
					case Tile.Yellow: // LeftLeaner
						Block[0, 1] = Tile.Yellow;
						Block[1, 1] = Tile.Yellow;
						Block[1, 2] = Tile.Yellow;
						Block[2, 2] = Tile.Yellow;
						break;
				}
			}

			public Piece(int x, int y)
				: this()
			{
				Location = new Point(x, y);
			}

			public Piece Rotate()
			{
				var s = new Piece();
				for (int i = 0; i < 4; ++i)
					for (int j = 0; j < 4; ++j)
						s.Block[3 - j, i] = Block[i, j];

				s.Location = Location;
				return s;
			}

			public Piece Move(int x, int y)
			{
				var s = new Piece();
				for (int i = 0; i < 4; ++i)
					for (int j = 0; j < 4; ++j)
						s.Block[i, j] = Block[i, j];

				s.Location = Location;
				s.Location.X += x;
				s.Location.Y += y;
				return s;
			}
		}

		void NextPiece()
		{
			_map.Add(_currPiece);
			int x = (_map.Width / 2 - 2);
			_currPiece = _nextPiece;
			if (_map.CheckCollision(_currPiece))
				NewGame();

			_nextPiece = new Piece(x, 0);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (!_timer.Enabled)
			{
				_map.DrawRandom(e.Graphics);
				_map.DrawNext(e.Graphics, new Piece(), BackColor);
				return;
			}

			_map.Draw(e.Graphics);
			_map.DrawNext(e.Graphics, _nextPiece, BackColor);
			_map.DrawPiece(e.Graphics, _currPiece);
		}

		void OnTick(object sender, EventArgs e)
		{
			// clear full raws if empty
			if (_map.RemoveRows())
				Invalidate();

			// check collision
			Piece tmpPiece = _currPiece.Move(0, 1);
			if (!_map.CheckCollision(tmpPiece))
				_currPiece = tmpPiece;
			else
				NextPiece();

			Invalidate();
		}
	}
}
