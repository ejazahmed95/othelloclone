namespace RangerRPG.Core {
	public static class Utils {
		public static bool TryConvertVal<TA, TB>(TA obj, out TB returnVal) {
			if (obj is TB b) {
				returnVal = b;
				return true;
			}
			returnVal = default(TB);
			return false;
		}
	}
}