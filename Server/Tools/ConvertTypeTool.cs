using System;

namespace Server.Tools
{
    class ConvertTypeTool
    {
        #region byte与int互转

        static public void Int32ToSpan(int value, Span<byte> span, int offset)
        {
            span[3 + offset] = (byte)((value >> 24) & 0xFF);
            span[2 + offset] = (byte)((value >> 16) & 0xFF);
            span[1 + offset] = (byte)((value >> 8) & 0xFF);
            span[0 + offset] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// 将int数值转换为占四个字节的byte数组，
        /// 本方法适用于(低位在前，高位在后)的顺序。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public byte[] Int32ToByteArray(int value)
        {
            byte[] src = new byte[4];
            src[3] = (byte)((value >> 24) & 0xFF);
            src[2] = (byte)((value >> 16) & 0xFF);
            src[1] = (byte)((value >> 8) & 0xFF);
            src[0] = (byte)(value & 0xFF);
            return src;
        }

        /// <summary>
        /// 把4个字节的byte数组转存到int32类型的数据中
        /// 本方法适用于(低位在前，高位在后)的顺序。
        /// </summary>
        /// <param name="arry">4个字节大小的byte数组</param>
        /// <returns></returns>
        static public int ByteArrayToInt32(byte[] src)
        {
            int value;
            value = (int)((src[0] & 0xFF)
                | ((src[1] & 0xFF) << 8)
                | ((src[2] & 0xFF) << 16)
                | ((src[3] & 0xFF) << 24));
            return value;
        }

        /// <summary>
        /// 把4个字节的ReadOnlySpan转存到int32类型的数据中
        /// 本方法适用于(低位在前，高位在后)的顺序。
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        static public int ByteArrayToInt32(ReadOnlySpan<byte> src)
        {
            int value;
            value = (int)((src[0] & 0xFF)
                | ((src[1] & 0xFF) << 8)
                | ((src[2] & 0xFF) << 16)
                | ((src[3] & 0xFF) << 24));
            return value;
        }

        /// <summary>
        /// 把两个ReadOnlySpan转存到int32类型的数据中
        /// 本方法适用于(低位在前，高位在后)的顺序。
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        static public int ByteArrayToInt32(ReadOnlySpan<byte> oneSpan, ReadOnlySpan<byte> twoSpan)
        {
            int value = 0;
            for (int i = 0; i < oneSpan.Length; i++)
            {
                value = value | (int)((oneSpan[i] & 0xFF) << 8 * i);
            }
            for (int i = 0; i < twoSpan.Length; i++)
            {
                value = value | (int)((twoSpan[i] & 0xFF) << 8 * (i + oneSpan.Length));
            }
            return value;
        }

        /// <summary>
        /// 将四个byte转存到int数据中 
        /// 本方法适用于(低位在前，高位在后)的顺序。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        static public int ByteArrayToInt32(byte a, byte b, byte c, byte d)
        {
            int value;
            value = (int)((a & 0xFF)
                | ((b & 0xFF) << 8)
                | ((c & 0xFF) << 16)
                | ((d & 0xFF) << 24));
            return value;
        }

        #endregion

        #region

        static public void LongToSpan(long value, Span<byte> span, int offset)
        {
            long temp = value;
            for (int i = 0; i < 8; i++)
            {
                span[i + offset] = (byte)((temp >> i * 8) & 0xFF);
            }
        }

        /// <summary>
        /// long转字节数组
        /// 低位在前，高位在后
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public byte[] LongToByteArray(long value)
        {
            long temp = value;
            byte[] array = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                array[i] = (byte)((temp >> i * 8) & 0xFF);
            }
            return array;
        }

        /// <summary>
        /// ReadOnlySpan转long
        /// 低位在前，高位在后
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        static public long ByteArrayToLong(ReadOnlySpan<byte> span)
        {
            long value = 0;
            for (int i = 0; i < 8; i++)
            {
                value = value | ((long)(span[i] & 0xFF) << 8 * i);
            }
            return value;
        }

        /// <summary>
        /// ReadOnlySpan转long
        /// 低位在前，高位在后
        /// </summary>
        /// <param name="oneSpan"></param>
        /// <param name="twoSpan"></param>
        /// <returns></returns>
        static public long ByteArrayToLong(ReadOnlySpan<byte> oneSpan, ReadOnlySpan<byte> twoSpan)
        {
            long value = 0;
            for (int i = 0; i < oneSpan.Length; i++)
            {
                value = value | ((long)(oneSpan[i] & 0xFF) << 8 * i);
            }
            for (int i = 0; i < twoSpan.Length; i++)
            {
                value = value | ((long)(twoSpan[i] & 0xFF) << 8 * (i + oneSpan.Length));
            }
            return value;
        }

        #endregion
    }
}
