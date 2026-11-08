public static class Units
{

    private const float PsiToBarConversionFactor = 0.0689476f;
    public static float PsiToBar(float psi)
    {
        return psi * PsiToBarConversionFactor;
    }

    public static float BarToPsi(float bar)
    {
        return bar / PsiToBarConversionFactor;
    }
}