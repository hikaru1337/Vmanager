using Aspose.PSD;
using Aspose.PSD.FileFormats.Png;
using Aspose.PSD.FileFormats.Psd;
using Aspose.PSD.FileFormats.Psd.Layers;
using Aspose.PSD.FileFormats.Psd.Layers.FillSettings;
using Aspose.PSD.FileFormats.Psd.Layers.SmartObjects;
using Aspose.PSD.ImageLoadOptions;
using Aspose.PSD.ImageOptions;
using DarlingDb;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.IO.Pipes;
using VManager.db.Model;

namespace VManager.AsposePSD.DatesPhotoshop
{
    public class VstarsDates
    {
        private readonly static PsdLoadOptions loadOptions = new PsdLoadOptions()
        {
            LoadEffectsResource = true,
            UseDiskForLoadEffectsResource = true,
        };

        public static MemoryStream RenderForYou(string VtuberName)
        {
            using (var db = new Db())
            {
                License license = new License();
                license.SetLicense("Key3.lic");
                var Vtuber = db.Vtuber.Include(x => x.Dates).FirstOrDefault(x => x.Name == VtuberName);
                var Vtuber_followers = GetFollowers.Followers(Vtuber.TwitchId);


                string ThisDir = AppDomain.CurrentDomain.BaseDirectory;
                string inputFile = ThisDir + "d 2.3 [Light].psd";

                var data = File.OpenRead(inputFile);

                using (var PSD = (PsdImage)Image.Load(data, loadOptions))
                {
                    var VtuberSubsUpdate = db.Vtuber.FirstOrDefault(x => x.Id == Vtuber.Id);

                    using (var TsunyaSmartObject = (SmartObjectLayer)PSD.Layers.FirstOrDefault(x => x.DisplayName == $"{Vtuber.Name} [SmartObject]"))
                    {
                        if (Vtuber.Dates.Count == 0 || Vtuber.Dates.ElementAt(0).Date_Description == "Offline")
                        {
                            using (var TsunyaOffline = PSD.Layers.FirstOrDefault(x => x.DisplayName == $"{Vtuber.Name} Offline"))
                            {
                                TsunyaOffline.IsVisible = true;
                                TsunyaSmartObject.IsVisible = false;


                                string dir = $"{AppDomain.CurrentDomain.BaseDirectory}1.png";
                                TsunyaOffline.Save(dir, new PngOptions());

                                TsunyaOffline.Dispose();
                                
                            }
                        }
                        return null;
                        //return ReadVtuberCard(PSD, Vtuber, Vtuber_followers.Total, true);
                    }
                }
            }
        }


        public static MemoryStream Render()
        {
            using (var db = new Db())
            {
                License license = new License();
                license.SetLicense("Key3.lic");

                var Tsunya = db.Vtuber.Include(x => x.Dates).FirstOrDefault(x=>x.Name == "Tsunya");
                var KraNf = db.Vtuber.Include(x => x.Dates).FirstOrDefault(x => x.Name == "KraNf");
                var Pewa = db.Vtuber.Include(x => x.Dates).FirstOrDefault(x => x.Name == "Pewa");
                var Aya = db.Vtuber.Include(x => x.Dates).FirstOrDefault(x => x.Name == "Aya");
                var xXxpososu = db.Vtuber.Include(x => x.Dates).FirstOrDefault(x => x.Name == "xXxpososu");

                var Tsunya_followers = GetFollowers.Followers(Tsunya.TwitchId);
                var X_followers = GetFollowers.Followers(xXxpososu.TwitchId);
                var Pewa_followers = GetFollowers.Followers(Pewa.TwitchId);
                var Kranf_followers = GetFollowers.Followers(KraNf.TwitchId);
                var Aya_followers = GetFollowers.Followers(Aya.TwitchId);


                string ThisDir = AppDomain.CurrentDomain.BaseDirectory;
                string inputFile = ThisDir + "d 2.3 [Light].psd";

                using (var PSD = (PsdImage)Image.Load(inputFile, loadOptions))
                {
                    ReadVtuberCard(PSD, Tsunya, Tsunya_followers.Total);
                    ReadVtuberCard(PSD, KraNf, X_followers.Total);
                    ReadVtuberCard(PSD, Pewa, Pewa_followers.Total);
                    ReadVtuberCard(PSD, Aya, Aya_followers.Total);
                    ReadVtuberCard(PSD, xXxpososu, X_followers.Total);

                    var textLayer = (TextLayer)PSD.Layers.FirstOrDefault(x=>x.DisplayName == "Date");
                    textLayer.TextData.Items[0].Paragraph.Justification = JustificationMode.Left;
                    var Start = new DateTime();
                    var End = new DateTime();
                    CalculateData(ref Start, ref End);

                    textLayer.UpdateText($"{Start.ToString("dd.MM")} - {End.ToString("dd.MM")}");

                    var opt = new PngOptions() { ColorType = PngColorType.TruecolorWithAlpha, CompressionLevel = 9, BitDepth = 8, Progressive = true, FilterType = PngFilterType.Adaptive };

                    xXxpososu.LastSubs = X_followers.Total;
                    Tsunya.LastSubs = Tsunya_followers.Total;
                    Pewa.LastSubs = Pewa_followers.Total;
                    Aya.LastSubs = Aya_followers.Total;
                    KraNf.LastSubs = Kranf_followers.Total;

                    db.SaveChangesAsync();


                    MemoryStream pptStream = new MemoryStream();
                    PSD.Save(pptStream, new PngOptions());
                    
                    return pptStream;
                    //PSD.Save(ThisDir + $"Result {DateTime.Now.ToString("ss HH mm dd MM yy")}.png", new BmpOptions());
                }
            }
        }

        static void CalculateData(ref DateTime startDateOfWeek, ref DateTime endDateOfWeek)
        {
            DateTime today = DateTime.Today;
            int daysToMonday = ((int)today.DayOfWeek - 1 + 7) % 7;
            startDateOfWeek = today.AddDays(-daysToMonday);
            endDateOfWeek = startDateOfWeek.AddDays(6);
        }



        public static MemoryStream ReadVtuberCard(PsdImage ListSmartOjbectImage, Vtuber Vtuber, ulong CountSubscribe, bool save = false)
        {
            using (var db = new Db())
            {
                var VtuberSubsUpdate = db.Vtuber.FirstOrDefault(x => x.Id == Vtuber.Id);

                using (var TsunyaSmartObject = (SmartObjectLayer)ListSmartOjbectImage.Layers.FirstOrDefault(x=>x.DisplayName == $"{Vtuber.Name} [SmartObject]"))
                {
                    if (Vtuber.Dates.Count == 0 || Vtuber.Dates.ElementAt(0).Date_Description == "Offline")
                    {
                        using (var TsunyaOffline = ListSmartOjbectImage.Layers.FirstOrDefault(x=>x.DisplayName == $"{Vtuber.Name} Offline"))
                        {
                            TsunyaOffline.IsVisible = true;
                            TsunyaSmartObject.IsVisible = false;

                            if (save)
                            {
                                //MemoryStream pptStream = new MemoryStream();
                                string dir = $"{AppDomain.CurrentDomain.BaseDirectory}1.png";
                                TsunyaOffline.Save(dir, new PngOptions());

                                return null;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }

                    using (var TsunyaSmartObjectImage = (PsdImage)TsunyaSmartObject.LoadContents(loadOptions))
                    {
                        var subscribeCountLayer = TsunyaSmartObjectImage.Layers.FirstOrDefault(x => x.DisplayName == "SubscriberCount");
                        var susbcribeCountText = (TextLayer)subscribeCountLayer;
                        susbcribeCountText.TextData.Items[0].Paragraph.Justification = JustificationMode.Left;
                        string subscribeCountText = HumanizeSubscribe.Humanize(CountSubscribe);
                        susbcribeCountText.UpdateText(subscribeCountText, Color.FromArgb(23, 23, 23));

                        var defaultColor = Color.FromArgb(34, 34, 34);


                        var RoundOldSubscriber = RoundCountSubscribers.RoundToNearestCircularNumber(Vtuber.LastSubs);
                        var RoundNewSubscriber = RoundCountSubscribers.RoundToNearestCircularNumber(CountSubscribe);

                        if (RoundNewSubscriber > RoundNewSubscriber)
                        {
                            defaultColor = Color.FromArgb(Convert.ToInt32(Vtuber.Color, 16));
                            VtuberSubsUpdate.LastSubs = CountSubscribe;
                            db.Vtuber.Update(VtuberSubsUpdate);
                            db.SaveChangesAsync();
                        }

                        var Gradient = subscribeCountLayer.BlendingOptions.AddGradientOverlay();
                        Gradient.Settings.ColorPoints = new[] { new GradientColorPoint() { Color = defaultColor, Location = 4096,MedianPointLocation = 0 },
                                                            new GradientColorPoint() { Color = Color.FromArgb(23,23,23), Location = 1024 + 512, MedianPointLocation = 60 } };


                        for (int i = 0; i < Vtuber.Dates.Count; i++)
                            NewsRead(TsunyaSmartObjectImage, Vtuber.Dates.ElementAt(i), i + 1);


                        if (save)
                        {
                            MemoryStream pptStream = new MemoryStream();
                            
                            TsunyaSmartObject.Save(pptStream, new PngOptions());
                            return pptStream;
                        }
                        else
                        {
                            TsunyaSmartObject.ReplaceContents(TsunyaSmartObjectImage);
                            return null;
                        }
                    }
                }
               
            }
        }

        public static void NewsRead(PsdImage ListSmartOjbectImage, Dates News, int count)
        {
            var NumberInWord = new Dictionary<int, string>() { { 1, "One" }, { 2, "Two" }, { 3, "Three" }, { 4, "Four" } };

            var DateFolder = (LayerGroup)ListSmartOjbectImage.Layers.FirstOrDefault(x => x.Name == $"Date {NumberInWord[count]}");
            DateFolder.IsVisible = true;

            if(News.Important)
                DateFolder.BlendingOptions.Effects[0].IsVisible = true;

            var DateLayer = (TextLayer)ListSmartOjbectImage.Layers.FirstOrDefault(x=>x.DisplayName == $"Date {NumberInWord[count]} Text");
            DateLayer.TextData.Items[0].Paragraph.Justification = JustificationMode.Left;
            DateLayer.UpdateText(News.Date_Description);
        }
    }
}
