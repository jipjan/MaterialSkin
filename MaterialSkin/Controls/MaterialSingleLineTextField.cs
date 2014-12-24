﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialSingleLineTextField : Control, IMaterialControl
    {
        //Properties for managing the material design properties
        public int Depth { get; set; }
        public MaterialSkinManager SkinManager { get { return MaterialSkinManager.Instance; } }
        public MouseState MouseState { get; set; }

        public override string Text { get { return baseTextBox.Text; } set { baseTextBox.Text = value;  } }
        public string Hint { get; set; }

        private readonly AnimationManager animationManager;

        private readonly TextBox baseTextBox = new TextBox();
        public MaterialSingleLineTextField()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer, true);

            animationManager = new AnimationManager()
            {
                Increment = 0.06,
                AnimationType = AnimationType.EaseInOut,
                InterruptAnimation = false
            };
            animationManager.OnAnimationProgress += sender => Invalidate();

            baseTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = SkinManager.ROBOTO_REGULAR_11,
                ForeColor = SkinManager.GetMainTextColor(),
                Location = new Point(0, 0),
                Text = Text,
                Width = Width,
                Height = Height - 5
            };

            if (!Controls.Contains(baseTextBox) && !DesignMode)
            {
                Controls.Add(baseTextBox);
            }

            baseTextBox.GotFocus += (sender, args) => animationManager.StartNewAnimation(AnimationDirection.In);
            baseTextBox.LostFocus += (sender, args) => animationManager.StartNewAnimation(AnimationDirection.Out);
            baseTextBox.TextChanged += BaseTextBoxOnTextChanged;
            BackColorChanged += (sender, args) =>
            {
                baseTextBox.BackColor = BackColor;
                baseTextBox.ForeColor = SkinManager.GetMainTextColor();
            };
        }

        private void BaseTextBoxOnTextChanged(object sender, EventArgs eventArgs)
        {
            if (baseTextBox.Text != Hint && baseTextBox.Text == "")
            {
                baseTextBox.Text = Hint;
                baseTextBox.ForeColor = SkinManager.GetDisabledOrHintColor();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.Clear(Parent.BackColor);

            int lineY = baseTextBox.Bottom + 3;

            if (!animationManager.IsAnimating())
            {
                //No animation
                g.FillRectangle(baseTextBox.Focused ? SkinManager.PrimaryColorBrush : SkinManager.GetDividersBrush(), baseTextBox.Location.X, lineY, baseTextBox.Width, baseTextBox.Focused ? 2 : 1);
            }
            else
            {
                //Animate
                int animationWidth = (int) (baseTextBox.Width * animationManager.GetProgress());
                int halfAnimationWidth = animationWidth / 2;
                int animationStart = baseTextBox.Location.X + baseTextBox.Width / 2;

                //Unfocused background
                g.FillRectangle(SkinManager.GetDividersBrush(), baseTextBox.Location.X, lineY, baseTextBox.Width, 1);
            
                //Animated focus transition
                g.FillRectangle(SkinManager.PrimaryColorBrush, animationStart - halfAnimationWidth, lineY, animationWidth, 2);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            baseTextBox.Location = new Point(0, 0);
            baseTextBox.Width = Width;
            
            Height = baseTextBox.Height + 5;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            baseTextBox.BackColor = Parent.BackColor;
            baseTextBox.ForeColor = SkinManager.GetMainTextColor();
        }
    }
}
