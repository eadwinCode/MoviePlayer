using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace $safeprojectname$
{
    public class Effect1 : ShaderEffect
    {
        #region Constructors

        static Effect1()
        {
            _pixelShader.UriSource = Global.MakePackUri("effect1.ps");
        }

        public Effect1()
        {
            this.PixelShader = _pixelShader;

            // Update each DependencyProperty that's registered with a shader register.  This
            // is needed to ensure the shader gets sent the proper default value.
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(ColorFilterProperty);
        }

        #endregion

        #region Dependency Properties

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        // Brush-valued properties turn into sampler-property in the shader.
        // This helper sets "ImplicitInput" as the default, meaning the default
        // sampler is whatever the rendering of the element it's being applied to is.
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(Effect1), 0);



        public Color ColorFilter
        {
            get { return (Color)GetValue(ColorFilterProperty); }
            set { SetValue(ColorFilterProperty, value); }
        }

        // Scalar-valued properties turn into shader constants with the register
        // number sent into PixelShaderConstantCallback().
        public static readonly DependencyProperty ColorFilterProperty =
            DependencyProperty.Register("ColorFilter", typeof(Color), typeof(Effect1),
                    new UIPropertyMetadata(Colors.Yellow, PixelShaderConstantCallback(0)));

        #endregion

        #region Member Data

        private static PixelShader _pixelShader = new PixelShader();

        #endregion

    }
}
