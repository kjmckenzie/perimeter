﻿using Xamarin.Forms;

namespace Perimeter.Controls
{
    public class AnimatedText : StackLayout
    {
        private const string AnimationName = "AnimatedTextAnimation";

        public static readonly BindableProperty IsRunningProperty =
            BindableProperty.Create(nameof(IsRunning), typeof(bool), 
                typeof(AnimatedText), default(bool));

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), 
                typeof(AnimatedText),
                default(string));

        private Animation _animation;

        public AnimatedText()
        {
            Orientation = StackOrientation.Horizontal;
            Spacing = -1;
        }

        public bool IsRunning
        {
            get => (bool) GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsRunning) && IsEnabled)
            {
                if (IsRunning)
                    StartAnimation();
                else
                    StopAnimation();
            }

            if (propertyName == nameof(Text))
            {
                InitAnimation();
            }
        }

        private void InitAnimation()
        {  
            _animation = new Animation();
            Children.Clear();
            if (string.IsNullOrWhiteSpace(Text))
                return;
            
            var index = 0;
            foreach (var textChar in Text)
            {
                var label = new Label
                {
                    Text = textChar.ToString(),
                    TextColor = Color.FromHex("#092ABB"),
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions=LayoutOptions.Center,
                    FontSize = 24
                };

                Children.Add(label);

                var oneCharAnimationLength = (double) 1 / (Text.Length + 1);

                _animation.Add(index * oneCharAnimationLength, (index + 1) * oneCharAnimationLength,
                    new Animation(v => label.Scale = v, 1, 1.75, Easing.Linear));
                _animation.Add((index + 1) * oneCharAnimationLength, (index + 2) * oneCharAnimationLength,
                    new Animation(v => label.Scale = v, 1.75, 1, Easing.Linear));
                
                _animation.Add(index * oneCharAnimationLength, (index + 1) * oneCharAnimationLength,
                    new Animation(v => label.TranslationY = v, 0, -10, Easing.Linear));
                _animation.Add((index + 1) * oneCharAnimationLength, (index + 2) * oneCharAnimationLength,
                    new Animation(v => label.TranslationY = v, -10, 0, Easing.Linear));

                index++;
            }
        }


        public void Start()
        {
            StopAnimation();
            StartAnimation();
        }

        private void StartAnimation()
        {
            _animation.Commit(this, AnimationName, 16,
                (uint) Children.Count * 200,
                Easing.Linear
                );

            StartAnimation();
        }

        private void StopAnimation()
        {
            this.AbortAnimation(AnimationName);
        }
    }
}