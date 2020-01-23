public class NumeralUtils
{
    public static int CountDigits(int num) {
        int counter = 0;

        while (num > 0) {
            counter++;
            num /= 10;
        }

        return counter;
    }
}