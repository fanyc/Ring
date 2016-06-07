using System;

namespace System.Numerics
{
	public struct BigDecimal : IConvertible, IFormattable, IComparable, IComparable<BigDecimal>, IEquatable<BigDecimal>
	{
		public static readonly BigDecimal MinusOne = new BigDecimal(BigInteger.MinusOne, 0);
		public static readonly BigDecimal Zero = new BigDecimal(BigInteger.Zero, 0);
		public static readonly BigDecimal One = new BigDecimal(BigInteger.One, 0);
		
		private readonly BigInteger _unscaledValue;
		private readonly int _scale;
		
		public BigDecimal(double value)
		: this((decimal)value) { }
		
		public BigDecimal(float value)
		: this((decimal)value) { }
		
		public BigDecimal(decimal value)
		{
			var bytes = FromDecimal(value);
			
			var unscaledValueBytes = new byte[12];
			Array.Copy(bytes, unscaledValueBytes, unscaledValueBytes.Length);
            
			
			var unscaledValue = new BigInteger(unscaledValueBytes);
			var scale = bytes[14];
			
			if (bytes[15] == 128)
				unscaledValue *= BigInteger.MinusOne;
			
			_unscaledValue = unscaledValue;
			_scale = scale;
		}
		
		public BigDecimal(int value)
		: this(new BigInteger(value), 0) { }
		
		public BigDecimal(long value)
		: this(new BigInteger(value), 0) { }
		
		public BigDecimal(uint value)
		: this(new BigInteger(value), 0) { }
		
		public BigDecimal(ulong value)
		: this(new BigInteger(value), 0) { }
		
        
		public BigDecimal(BigInteger unscaledValue, int scale)
		{
			_unscaledValue = unscaledValue;
			_scale = scale;
		}
		
		public BigDecimal(byte[] value)
		{
			byte[] number = new byte[value.Length - 4];
			byte[] flags = new byte[4];
			
			Array.Copy(value, 0, number, 0, number.Length);
			Array.Copy(value, value.Length - 4, flags, 0, 4);
			
			_unscaledValue = new BigInteger(number);
			_scale = BitConverter.ToInt32(flags, 0);
		}
		
		// public bool IsEven { get { return _unscaledValue.IsEven; } }
		// public bool IsOne { get { return _unscaledValue.IsOne; } }
		// public bool IsPowerOfTwo { get { return _unscaledValue.IsPowerOfTwo; } }
		// public bool IsZero { get { return _unscaledValue.IsZero; } }
		// public int Sign { get { return _unscaledValue.Sign; } }
		
		public override string ToString()
		{
			var number = _unscaledValue.ToString();
			
			if (_scale > 0)
				return number.Insert(number.Length - _scale, ".");
			
			return number;
		}
		
		private static byte[] FromDecimal(decimal d)
		{
			byte[] bytes = new byte[16];
			
			int[] bits = decimal.GetBits(d);
			int lo = bits[0];
			int mid = bits[1];
			int hi = bits[2];
			int flags = bits[3];
			
			bytes[11] = (byte)lo;
			bytes[10] = (byte)(lo >> 8);
			bytes[9] = (byte)(lo >> 0x10);
			bytes[8] = (byte)(lo >> 0x18);
			bytes[7] = (byte)mid;
			bytes[6] = (byte)(mid >> 8);
			bytes[5] = (byte)(mid >> 0x10);
			bytes[4] = (byte)(mid >> 0x18);
			bytes[3] = (byte)hi;
			bytes[2] = (byte)(hi >> 8);
			bytes[1] = (byte)(hi >> 0x10);
			bytes[0] = (byte)(hi >> 0x18);
			bytes[12] = (byte)flags;
			bytes[13] = (byte)(flags >> 8);
			bytes[14] = (byte)(flags >> 0x10);
			bytes[15] = (byte)(flags >> 0x18);
			
			return bytes;
		}
		
		#region Operators
		
		public static bool operator ==(BigDecimal left, BigDecimal right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(BigDecimal left, BigDecimal right)
		{
			return !left.Equals(right);
		}
		
		public static bool operator >(BigDecimal left, BigDecimal right)
		{
			return (left.CompareTo(right) > 0);
		}
		
		public static bool operator >=(BigDecimal left, BigDecimal right)
		{
			return (left.CompareTo(right) >= 0);
		}
		
		public static bool operator <(BigDecimal left, BigDecimal right)
		{
			return (left.CompareTo(right) < 0);
		}
		
		public static bool operator <=(BigDecimal left, BigDecimal right)
		{
			return (left.CompareTo(right) <= 0);
		}
		
		public static bool operator ==(BigDecimal left, decimal right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(BigDecimal left, decimal right)
		{
			return !left.Equals(right);
		}
		
		public static bool operator >(BigDecimal left, decimal right)
		{
			return (left.CompareTo(right) > 0);
		}
		
		public static bool operator >=(BigDecimal left, decimal right)
		{
			return (left.CompareTo(right) >= 0);
		}
		
		public static bool operator <(BigDecimal left, decimal right)
		{
			return (left.CompareTo(right) < 0);
		}
		
		public static bool operator <=(BigDecimal left, decimal right)
		{
			return (left.CompareTo(right) <= 0);
		}
		
		public static bool operator ==(decimal left, BigDecimal right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(decimal left, BigDecimal right)
		{
			return !left.Equals(right);
		}
		
		public static bool operator >(decimal left, BigDecimal right)
		{
			return (left.CompareTo(right) > 0);
		}
		
		public static bool operator >=(decimal left, BigDecimal right)
		{
			return (left.CompareTo(right) >= 0);
		}
		
		public static bool operator <(decimal left, BigDecimal right)
		{
			return (left.CompareTo(right) < 0);
		}
		
		public static bool operator <=(decimal left, BigDecimal right)
		{
			return (left.CompareTo(right) <= 0);
		}

		private BigDecimal Upscale(int newScale) {
			if (newScale < _scale)
				throw new InvalidOperationException("Cannot upscale a BigDecimal to a smaller scale!");
			
			return new BigDecimal(_unscaledValue * BigInteger.Pow(10, newScale - _scale), newScale);
		}
		
		private static int SameScale(ref BigDecimal left, ref BigDecimal right) {
			int newScale = Math.Max(left._scale, right._scale);
			left = left.Upscale(newScale);
			right = right.Upscale(newScale);
			return newScale;
		}
		
		public static BigDecimal operator +(BigDecimal left, BigDecimal right) {
			int scale = SameScale(ref left, ref right);
			return new BigDecimal(left._unscaledValue + right._unscaledValue, scale);
		}
		
		public static BigDecimal operator -(BigDecimal left, BigDecimal right) {
			int scale = SameScale(ref left, ref right);
			return new BigDecimal(left._unscaledValue - right._unscaledValue, scale);
		}
		
		public static BigDecimal operator *(BigDecimal left, BigDecimal right) {
			BigInteger value = left._unscaledValue * right._unscaledValue;
			int scale = left._scale + right._scale;
			if (scale > int.MaxValue) {
				value /= BigInteger.Pow(10, scale - int.MaxValue);
				scale = int.MaxValue;
			}
			return new BigDecimal(value, scale);
		}
		
		public static double Log10(BigDecimal value)
		{
			int digit = value._unscaledValue.ToString().Length;
			double log = Math.Log10(new BigDecimal(value._unscaledValue, digit).ToType<double>());
			return log + (digit - value._scale);
		}

		public static BigDecimal Pow(BigDecimal left, BigDecimal right) {
            if(right == BigDecimal.Zero) return BigDecimal.One;
            if(right == BigDecimal.One) return left;
            if(right == BigDecimal.MinusOne) return -left;
            
 			double log = (right.ToType<double>() * (Log10(left) - left._scale));
			int scale = (int)Math.Floor(log);
			return (BigDecimal)BigInteger.Pow (10, scale) * (BigDecimal)Math.Exp((log - scale) / Math.Log10(Math.E));
		}
		
		#endregion
		
		#region Explicity and Implicit Casts
		
		public static implicit operator byte(BigDecimal value) { return value.ToType<byte>(); }
		public static implicit operator sbyte(BigDecimal value) { return value.ToType<sbyte>(); }
		public static implicit operator short(BigDecimal value) { return value.ToType<short>(); }
		public static implicit operator int(BigDecimal value) { return value.ToType<int>(); }
		public static implicit operator long(BigDecimal value) { return value.ToType<long>(); }
		public static implicit operator ushort(BigDecimal value) { return value.ToType<ushort>(); }
		public static implicit operator uint(BigDecimal value) { return value.ToType<uint>(); }
		public static implicit operator ulong(BigDecimal value) { return value.ToType<ulong>(); }
		public static implicit operator float(BigDecimal value) { return value.ToType<float>(); }
		public static implicit operator double(BigDecimal value) { return value.ToType<double>(); }
		public static implicit operator decimal(BigDecimal value) { return value.ToType<decimal>(); }
		public static implicit operator BigInteger(BigDecimal value)
		{
			var scaleDivisor = BigInteger.Pow(new BigInteger(10), value._scale);
			var scaledValue = BigInteger.Divide(value._unscaledValue, scaleDivisor);
			return scaledValue;
		}
		
		public static implicit operator BigDecimal(byte value) { return new BigDecimal(value); }
		public static implicit operator BigDecimal(sbyte value) { return new BigDecimal(value); }
		public static implicit operator BigDecimal(short value) { return new BigDecimal(value); }
		public static implicit operator BigDecimal(int value) { return new BigDecimal(value); }
		public static implicit operator BigDecimal(long value) { return new BigDecimal(value); }
		public static implicit operator BigDecimal(ushort value) { return new BigDecimal(value); }
		public static implicit operator BigDecimal(uint value) { return new BigDecimal(value); }
		public static implicit operator BigDecimal(ulong value) { return new BigDecimal(value); }
		public static implicit operator BigDecimal(float value) { return new BigDecimal(value); }
		public static implicit operator BigDecimal(double value) { return new BigDecimal(value); }
		public static implicit operator BigDecimal(decimal value) { return new BigDecimal(value); }
		public static implicit operator BigDecimal(BigInteger value) { return new BigDecimal(value, 0); }
		
		#endregion
		
		public T ToType<T>() where T : struct
		{
			return (T)((IConvertible)this).ToType(typeof(T), null);
		}
		
		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			var scaleDivisor = BigInteger.Pow(new BigInteger(10), this._scale);
			var remainder = BigInteger.Modulus(this._unscaledValue, scaleDivisor);
			var scaledValue = BigInteger.Divide(this._unscaledValue, scaleDivisor);
			
			if (scaledValue > new BigInteger(Math.Truncate(Decimal.MaxValue)))
				throw new ArgumentOutOfRangeException("value", "The value " + this._unscaledValue + " cannot fit into " + conversionType.Name + ".");
			
			var leftOfDecimal = BigInteger.ToDecimal(scaledValue);
			var rightOfDecimal = BigInteger.ToDecimal(remainder) / BigInteger.ToDecimal(scaleDivisor);
			
			var value = leftOfDecimal + rightOfDecimal;
			return Convert.ChangeType(value, conversionType);
		}
		
		public override bool Equals(object obj)
		{
			return ((obj is BigDecimal) && Equals((BigDecimal)obj));
		}
		
		public override int GetHashCode()
		{
			return _unscaledValue.GetHashCode() ^ _scale.GetHashCode();
		}
		
		#region IConvertible Members
		
		TypeCode IConvertible.GetTypeCode()
		{
			return TypeCode.Object;
		}
		
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}
		
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}
		
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot cast BigDecimal to Char");
		}
		
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot cast BigDecimal to DateTime");
		}
		
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}
		
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}
		
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}
		
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}
		
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}
		
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}
		
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this);
		}
		
		string IConvertible.ToString(IFormatProvider provider)
		{
			return Convert.ToString(this);
		}
		
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}
		
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}
		
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}
		
		#endregion
		
		#region IFormattable Members
		
		public string ToString(string format, IFormatProvider formatProvider)
		{
			throw new NotImplementedException();
		}
		
		public string ToUnit()
		{
			double log = Log10(this);
			if(log < 3.0f) return this.ToType<float>().ToString("0");
			return Math.Exp((log % 3.0) / 0.434294481903252).ToString("0.00") + (char)(log >= 3.0 ? 64 + ((int)log) / 3 : '\0');
		}
		
		#endregion
		
		#region IComparable Members
		
		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			
			if (!(obj is BigDecimal))
				throw new ArgumentException("Compare to object must be a BigDecimal", "obj");
			
			return CompareTo((BigDecimal)obj);
		}
		
		#endregion
		
		#region IComparable<BigDecimal> Members
		
		public int CompareTo(BigDecimal other)
		{
			var unscaledValueCompare = this._unscaledValue.CompareTo(other._unscaledValue);
			var scaleCompare = -this._scale.CompareTo(other._scale);
			
			// if both are the same value, return the value
			if (unscaledValueCompare == scaleCompare)
				return unscaledValueCompare;
			
			// if the scales are both the same return unscaled value
			if (scaleCompare == 0)
				return unscaledValueCompare;
			
			var scaledValue = BigInteger.Divide(this._unscaledValue, BigInteger.Pow(new BigInteger(10), this._scale));
			var otherScaledValue = BigInteger.Divide(other._unscaledValue, BigInteger.Pow(new BigInteger(10), other._scale));
			
			return scaledValue.CompareTo(otherScaledValue);
		}
		
		#endregion
		
		#region IEquatable<BigDecimal> Members
		
		public bool Equals(BigDecimal other)
		{
			return this._scale == other._scale && this._unscaledValue == other._unscaledValue;
		}
		
		#endregion
	}
}