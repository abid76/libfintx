using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using libfintx.FinTS.Data.Segment;
using libfintx.Logger.Log;

namespace libfintx.FinTS
{
    public static partial class Helper
    {

        /// <summary>
        /// Fill given <code>TANDialog</code> and wait for user to enter a TAN.
        /// </summary>
        /// <param name="BankCode"></param>
        /// <param name="pictureBox"></param>
        /// <param name="flickerImage"></param>
        /// <param name="flickerWidth"></param>
        /// <param name="flickerHeight"></param>
        /// <param name="renderFlickerCodeAsGif"></param>
        public static async Task<string> WaitForTanAsync(FinTsClient client, HBCIDialogResult dialogResult, TANDialog tanDialog)
        {
            List<string> rawSegments = SplitEncryptedSegments(dialogResult.RawData);
            List<Segment> segments = new List<Segment>();
            foreach (var item in rawSegments)
            {
                var segment = Parse_Segment(item);
                if (segment != null)
                    segments.Add(segment);
            }

            var BankCode_ = "HIRMS" + Parse_String(dialogResult.RawData, "'HIRMS", "'");
            string[] values = BankCode_.Split('+');
            foreach (var item in values)
            {
                if (!item.StartsWith("HIRMS"))
                    TransactionConsole.Output = item.Replace("::", ": ");
            }

            var HITAN = segments.FirstOrDefault(s => s.Name == "HITAN");
            var HITAN_value = HITAN?.Value;
            var HITAN_challenge = HITAN.DataElements.Count > 4 ? HITAN.DataElements[4] : null;

            var processes = BPD.HITANS.Where(h => h.Version == client.HITANS).SelectMany(t => t.TanProcesses);

            var processname = string.Empty;

            foreach (var item in processes)
            {
                if (item.TanCode == Convert.ToInt32(client.HIRMS))
                    processname = item.Name;
            }

            Log.Write($"Processing TAN process '{processname}' ...");

            string HITANFlicker = string.Empty;

            // Smart-TAN plus optisch
            // chipTAN optisch
            if (processname.Equals("Smart-TAN plus optisch") || processname.Contains("chipTAN optisch"))
            {
                HITANFlicker = HITAN_value;
            }

            // chipTAN optisch
            if (processname.Contains("chipTAN optisch"))
            {
                string FlickerCode = string.Empty;

                FlickerCode = "CHLGUC" + Helper.Parse_String(HITAN_value, "CHLGUC", "CHLGTEXT") + "CHLGTEXT";

                FlickerCode flickerCode = new FlickerCode(FlickerCode);
                flickerCodeRenderer = new FlickerRenderer(flickerCode.Render(), tanDialog.PictureBox);
                if (!tanDialog.RenderFlickerCodeAsGif)
                {
                    RUN_flickerCodeRenderer();

                    Action action = STOP_flickerCodeRenderer;
                    TimeSpan span = new TimeSpan(0, 0, 0, 50);

                    ThreadStart start = delegate { RunAfterTimespan(action, span); };
                    Thread thread = new Thread(start);
                    thread.Start();
                }
                else
                {
                    tanDialog.FlickerImage = flickerCodeRenderer.RenderAsGif(tanDialog.FlickerWidth, tanDialog.FlickerHeight);
                }
            }

            // Smart-TAN plus optisch
            if (processname.Equals("Smart-TAN plus optisch"))
            {
                HITANFlicker = HITAN_value.Replace("?@", "??");

                string FlickerCode = string.Empty;

                String[] values__ = HITANFlicker.Split('@');

                int ii = 1;

                foreach (var item in values__)
                {
                    ii = ii + 1;

                    if (ii == 4)
                        FlickerCode = item;
                }

                FlickerCode flickerCode = new FlickerCode(FlickerCode.Trim());
                flickerCodeRenderer = new FlickerRenderer(flickerCode.Render(), tanDialog.PictureBox);
                if (!tanDialog.RenderFlickerCodeAsGif)
                {
                    RUN_flickerCodeRenderer();

                    Action action = STOP_flickerCodeRenderer;
                    TimeSpan span = new TimeSpan(0, 0, 0, 50);

                    ThreadStart start = delegate { RunAfterTimespan(action, span); };
                    Thread thread = new Thread(start);
                    thread.Start();
                }
                else
                {
                    tanDialog.FlickerImage = flickerCodeRenderer.RenderAsGif(tanDialog.FlickerWidth, tanDialog.FlickerHeight);
                }
            }

            // Smart-TAN photo
            if (processname.Equals("Smart-TAN photo") || processname.Equals("photoTAN-Verfahren") || processname.Equals("photoTAN"))
            {
                var PhotoCode = HITAN_challenge;

                var mCode = new MatrixCode(PhotoCode);

                tanDialog.MatrixImage = mCode.CodeImage;
                mCode.Render(tanDialog.PictureBox);
            }

            return await tanDialog.WaitForTanAsync();
        }
    }
}
