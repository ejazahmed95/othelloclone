namespace RangerRPG.Core {
    public static class MathUtils {
        public static bool InRange(int value, int lower, int upper) {
            return value >= lower && value < upper;
        } 
    }
}