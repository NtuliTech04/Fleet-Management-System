using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bytescout.BarCodeReader;

namespace FleetTours___Application
{
    public class Barcode
    {
        public static BarcodeTypeSelector GetBarcodeTypeToFindFromCombobox(string barType)
        {
            string selectedItemText = barType.Trim().ToUpper();
            BarcodeTypeSelector barcodeTypeToScan = new BarcodeTypeSelector();
            selectedItemText.IndexOf("ALL BARCODES");
            barcodeTypeToScan.SetAll();
            return barcodeTypeToScan;
        }
    }
}