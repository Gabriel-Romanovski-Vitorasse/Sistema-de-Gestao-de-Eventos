using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SistemaDeGestão.Controls;

public enum PillKind { Neutral, Success, Warning, Info, Danger }

/// <summary>Etiqueta de status arredondada ("Publicado", "Esgotado"...).</summary>
public class StatusPill : Control
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(StatusPill), new PropertyMetadata(""));
    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(PillKind), typeof(StatusPill), new PropertyMetadata(PillKind.Neutral));

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public PillKind Kind { get => (PillKind)GetValue(KindProperty); set => SetValue(KindProperty, value); }
}

/// <summary>Aviso informativo com ícone "info".</summary>
public class Notice : Control
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(Notice), new PropertyMetadata(""));
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
}

/// <summary>Círculo com iniciais.</summary>
public class Avatar : Control
{
    public static readonly DependencyProperty InitialsProperty =
        DependencyProperty.Register(nameof(Initials), typeof(string), typeof(Avatar), new PropertyMetadata(""));
    public string Initials { get => (string)GetValue(InitialsProperty); set => SetValue(InitialsProperty, value); }
}

/// <summary>Avatar + nome + descrição.</summary>
public class PersonChip : Control
{
    public static readonly DependencyProperty InitialsProperty =
        DependencyProperty.Register(nameof(Initials), typeof(string), typeof(PersonChip), new PropertyMetadata(""));
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(PersonChip), new PropertyMetadata(""));
    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(PersonChip), new PropertyMetadata(""));

    public string Initials { get => (string)GetValue(InitialsProperty); set => SetValue(InitialsProperty, value); }
    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Subtitle { get => (string)GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
}

/// <summary>Campo de formulário: rótulo + controle.</summary>
public class Field : ContentControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(Field), new PropertyMetadata(""));
    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
}

/// <summary>Título (17) e subtítulo opcional (12) de um card.</summary>
public class SectionHeader : Control
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(SectionHeader), new PropertyMetadata(""));
    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(SectionHeader), new PropertyMetadata(null));

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string? Subtitle { get => (string?)GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
}

/// <summary>Card de indicador (rótulo, valor grande e legenda).</summary>
public class StatCard : Control
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(StatCard), new PropertyMetadata(""));
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(StatCard), new PropertyMetadata(""));
    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register(nameof(Caption), typeof(string), typeof(StatCard), new PropertyMetadata(""));

    public string Label { get => (string)GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public string Caption { get => (string)GetValue(CaptionProperty); set => SetValue(CaptionProperty, value); }
}

/// <summary>Rodapé de tabela: "Mostrando 1–6 de N" + páginas.</summary>
public class Pager : Control
{
    public static readonly DependencyProperty SummaryProperty =
        DependencyProperty.Register(nameof(Summary), typeof(string), typeof(Pager), new PropertyMetadata(""));
    public string Summary { get => (string)GetValue(SummaryProperty); set => SetValue(SummaryProperty, value); }
}

/// <summary>StackPanel com espaçamento uniforme entre filhos (equivalente ao "gap" do Figma).</summary>
public class SpacedStackPanel : Panel
{
    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(SpacedStackPanel),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(SpacedStackPanel),
            new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public double Spacing { get => (double)GetValue(SpacingProperty); set => SetValue(SpacingProperty, value); }
    public Orientation Orientation { get => (Orientation)GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }

    protected override Size MeasureOverride(Size available)
    {
        bool vertical = Orientation == Orientation.Vertical;
        var childAvail = vertical ? new Size(available.Width, double.PositiveInfinity)
                                  : new Size(double.PositiveInfinity, available.Height);
        double main = 0, cross = 0; int visible = 0;
        foreach (UIElement child in InternalChildren)
        {
            child.Measure(childAvail);
            if (child.Visibility == Visibility.Collapsed) continue;
            visible++;
            var s = child.DesiredSize;
            main += vertical ? s.Height : s.Width;
            cross = Math.Max(cross, vertical ? s.Width : s.Height);
        }
        if (visible > 1) main += Spacing * (visible - 1);
        return vertical ? new Size(cross, main) : new Size(main, cross);
    }

    protected override Size ArrangeOverride(Size final)
    {
        bool vertical = Orientation == Orientation.Vertical;
        double offset = 0;
        foreach (UIElement child in InternalChildren)
        {
            if (child.Visibility == Visibility.Collapsed) continue;
            var s = child.DesiredSize;
            if (vertical)
            {
                child.Arrange(new Rect(0, offset, final.Width, s.Height));
                offset += s.Height + Spacing;
            }
            else
            {
                child.Arrange(new Rect(offset, 0, s.Width, final.Height));
                offset += s.Width + Spacing;
            }
        }
        return final;
    }
}

/// <summary>Converte uma altura em CornerRadius circular.</summary>
public sealed class HalfCornerConverter : IValueConverter
{
    public static readonly HalfCornerConverter Instance = new();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => new CornerRadius(value is double d ? d / 2 : 0);
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
