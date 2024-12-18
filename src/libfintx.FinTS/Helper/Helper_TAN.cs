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
        /// <param name="client"></param>
        /// <param name="dialogResult"></param>
        /// <param name="tanDialog"></param>
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

            var HITAN = segments.FirstOrDefault(s => s.Name == "HITAN");
            var HITAN_value = HITAN?.Value;
            var HITAN_challenge = HITAN.DataElements.Count > 4 ? HITAN.DataElements[4].Value : null;

            var processes = BPD.HITANS.Where(h => h.Version == client.HktanVersion).SelectMany(t => t.TanProcesses);

            var processname = string.Empty;

            foreach (var item in processes)
            {
                if (item.TanCode == Convert.ToInt32(client.TanProcessCode))
                    processname = item.Name;
            }

            Log.Write($"Processing TAN process '{processname}' ...");

            // Smart-TAN photo
            if (processname.Equals("Smart-TAN photo") || processname.Equals("photoTAN-Verfahren") || processname.Equals("photoTAN"))
            {
                var PhotoCode = HITAN_challenge;

                var mCode = new MatrixCode(PhotoCode);
                tanDialog.MatrixImage = mCode.Image;
            }

            return await tanDialog.WaitForTanAsync();
        }
    }
}
