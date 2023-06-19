namespace ZFrame
{
    public class Format
    {
        /// <summary>转换数字为k，m，b</summary>
        public static string numToKMB(float num,int point=0)
        {
            var str = "";
            if (num >= 10000000000)
            {
                num = num / 1000000000;
                str=num.ToString("#0.0")+"b";
            }
            else if (num >= 10000000)
            {
                 num =num / 1000000;
                str = num.ToString("#0.0") + "m";
            }
            else if (num >= 10000)
            {
                num= num / 1000;
                str = num.ToString("#0.0") + "k";
            }else{
                str=num.ToString();
            }
            return str;
            // if (typeof(T) == typeof(float))
            // {
            //     return  as T;
            // }else if(typeof(T)==typeof(string)){
            //     return "" as T;
            // }
        }
    }
}