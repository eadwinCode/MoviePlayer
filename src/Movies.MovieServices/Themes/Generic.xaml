<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:Componentdock="clr-namespace:Movies.MovieServices.Services">
    <Style TargetType="{x:Type Componentdock:ComponentDocker}">
        <Setter Property="Background" Value="Transparent"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Componentdock:ComponentDocker">
                    <Grid HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
                        <Grid.Background>
                            <LinearGradientBrush EndPoint="0.5,1" MappingMode="RelativeToBoundingBox" StartPoint="0.5,0">
                                <GradientStop Color="Transparent"/>
                                <GradientStop Color="#7F000000"/>
                            </LinearGradientBrush>
                        </Grid.Background>
                        <Border VerticalAlignment="Center"
                                Background="#CC000000" 
                                HorizontalAlignment="Center"  CornerRadius="5" BorderThickness=".8"
                                BorderBrush="{DynamicResource Theme.HighlightColor}"
                                Height="{TemplateBinding DialogHeight}"
                                Width="{TemplateBinding DialogWidth}">
                            <DockPanel>
                                <Border Padding="3" DockPanel.Dock="Top" CornerRadius="0" Background="{DynamicResource Theme.HighlightColor}"
                                        Grid.ColumnSpan="2">
                                    <TextBlock Padding="5" TextAlignment="Left" FontWeight="Regular" FontSize="16"
                                               Foreground="White" VerticalAlignment="Center"
                                               Text="{TemplateBinding DialogTitle}"/>
                                </Border>
                                <Grid DockPanel.Dock="Bottom">
                                    <StackPanel Grid.Row="4"  HorizontalAlignment="Right" Margin="5" Orientation="Horizontal">
                                        <Button Margin="2" Command="{x:Static Componentdock:ComponentDocker.OkCommand}" Height="30"
                                                Width="80" HorizontalAlignment="Right" Style="{DynamicResource dialogbtn}">
                                            <TextBlock Text="{TemplateBinding OKButtonText}"  HorizontalAlignment="Center"  
                                                       VerticalAlignment="Center" FontWeight="DemiBold"
                                                       FontSize="15" Foreground="OldLace"/>
                                        </Button>
                                        <Button Margin="2" Command="{x:Static Componentdock:ComponentDocker.CancelCommand}"  Height="30"
                                                Width="80" HorizontalAlignment="Right" Style="{DynamicResource dialogbtn}">
                                            <TextBlock Text="{TemplateBinding CancelButtonText}"  HorizontalAlignment="Center" 
                                                       VerticalAlignment="Center" FontWeight="DemiBold"
                                                       FontSize="15" Foreground="OldLace"/>
                                        </Button>
                                    </StackPanel>
                                </Grid>
                                <Grid Margin="1" >
                                    <ContentPresenter Content="{Binding Content,RelativeSource={RelativeSource AncestorType=Componentdock:ComponentDocker}}"/>
                                </Grid>
                            </DockPanel>
                        </Border>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
</ResourceDictionary>