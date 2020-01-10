public static class AngleUtils
{
    /// <summary>
    /// Convert an angle from the range of 0 to 360 degrees,
    /// to the same angle at the range of -180 to 180 degrees.
    /// </summary>
    /// <param name="angle">The angle to convert</param>
    /// <returns>A converted angle from -180 to 180 degrees.</returns>
    public static float TangentiateAngle(float angle) {
        return (angle > 180) ? angle - 360 : angle;
    }
}