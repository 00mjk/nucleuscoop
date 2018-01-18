﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Nucleus.Gaming;
using Nucleus.Gaming.Coop;
using Nucleus.Gaming.Package;
using Nucleus.Gaming.Platform.Windows.Controls;

namespace Nucleus.Gaming.Coop
{
    /// <summary>
    /// 
    /// </summary>
    public class GameControl : UserControl, IDynamicSized, IRadioControl
    {
        public UserGameInfo UserGameInfo { get; private set; }

        private PictureBox picture;
        private Label title;
        public string TitleText { get; set; }

        public GameControl(UserGameInfo userGame)
        {
            UserGameInfo = userGame;

            picture = new PictureBox();
            picture.SizeMode = PictureBoxSizeMode.StretchImage;

            title = new Label();
            if (userGame == null)
            {
                title.Text = "No games";
            }
            else
            {
                title.Text = GameManager.Instance.NameManager.GetGameName(userGame.GameID);
            }
            TitleText = title.Text;

            BackColor = Color.FromArgb(30, 30, 30);
            Size = new Size(200, 52);

            Controls.Add(picture);
            Controls.Add(title);

            DPIManager.Register(this);
        }


        ~GameControl()
        {
            DPIManager.Unregister(this);
        }

        public void UpdateSize(float scale)
        {
            if (IsDisposed)
            {
                DPIManager.Unregister(this);
                return;
            }

            SuspendLayout();

            int border = DPIManager.Adjust(4, scale);
            int dborder = border * 2;

            picture.Location = new Point(border, border);
            picture.Size = new Size(DPIManager.Adjust(44, scale), DPIManager.Adjust(44, scale));

            Height = DPIManager.Adjust(52, scale);

            Size labelSize = TextRenderer.MeasureText(TitleText, title.Font);
            float reservedSpaceLabel = this.Width - picture.Width;

            if (labelSize.Width > reservedSpaceLabel)
            {
                // make text smaller
                int charSize = TextRenderer.MeasureText("g", title.Font).Width;
                int toRemove = (int)((reservedSpaceLabel - labelSize.Width) / (float)charSize);
                toRemove = Math.Max(toRemove + 3, 7);
                title.Text = TitleText.Remove(TitleText.Length - toRemove, toRemove) + "...";
            }
            else
            {
                title.Text = TitleText;
            }
            title.Size = labelSize;

            float height = this.Height / 2.0f;
            float lheight = labelSize.Height / 2.0f;

            title.Location = new Point(picture.Width + picture.Left + border, (int)(height - lheight));

            ResumeLayout();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            Control c = e.Control;
            c.Click += C_Click;
            c.MouseEnter += C_MouseEnter;
            c.MouseLeave += C_MouseLeave;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateSize(DPIManager.Scale);
        }

        private void C_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        private void C_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void C_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        public Image Image
        {
            get { return this.picture.Image; }
            set { this.picture.Image = value; }
        }

        public override string ToString()
        {
            return Text;
        }

        private bool isSelected;
        public void RadioSelected()
        {
            BackColor = Color.FromArgb(80, 80, 80);
            isSelected = true;
        }

        public void RadioUnselected()
        {
            BackColor = Color.FromArgb(30, 30, 30);
            isSelected = false;
        }

        public void UserOver()
        {
            BackColor = Color.FromArgb(60, 60, 60);
        }

        public void UserLeave()
        {
            if (isSelected)
            {
                BackColor = Color.FromArgb(80, 80, 80);
            }
            else
            {
                BackColor = Color.FromArgb(30, 30, 30);
            }
        }
    }
}
