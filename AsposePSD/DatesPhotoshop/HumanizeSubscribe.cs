namespace VManager.AsposePSD.DatesPhotoshop
{
    public class HumanizeSubscribe
    {
        public static string Humanize(ulong number)
        {
            string numberinword = string.Empty;
            if (number < 1000)
                numberinword = number.ToString();
            else
            {
                float deliter = 1000f;
                float humanized = number / deliter;

                if (number % deliter == 0)
                {
                    numberinword = (number / deliter).ToString() + "k";
                }
                else
                {
                    numberinword = humanized.ToString(humanized >= 10000 ? "00.0" : "0.0") + "k";
                }
            }
            return numberinword.Replace(',', '.');
        }
    }
}
