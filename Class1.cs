using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using IntersonArray;
using IntersonArray.Imaging;
using IntersonArray.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Globalization;
using System.Management;
using IntersonTools.Graphic;


namespace exjobb
{
    class Class1 : Form
    {
        public Byte[,] buffer = new byte[100, 100];
        public void PrintValues1(StringCollection myCol)
        {
            foreach (Object obj in myCol)
                Console.WriteLine("   {0}", obj);
            Console.WriteLine();
        }

        public Capture Scan2D = new Capture();
        ScanConverter ScanConv = new ScanConverter();
        ImageBuilding ImageBuilding = new ImageBuilding();
        public void run()
        {
            System.Console.WriteLine("Hello World!");
            StringCollection mycolProbesName = new StringCollection();
            HWControls MyHwControls = new HWControls();
            MyHwControls.FindAllProbes(ref mycolProbesName);
            MyHwControls.FindMyProbe(0);

            HWControls.StartBmode();
            //HWControls.StartCFMmode();

            ushort[] aus = new ushort[ScanConverter.MAX_CFM_LINES * ScanConverter.MAX_RFSAMPLES];
            //scan2d.StartReadScan(ref buffer);
            //scan2d.StartCFMReadScan(ref buffer, ref aus);
            //scan2d.NewImageTick += new Capture.NewImageHandler(ImageRefresh);
            InitScan();
            MyHwControls.EnableHardButton();

            MyHwControls.HWButtonTickPressed += new HWControls.HWButtonHandlerPressed(WatchHWButtonPressed);
            
            int x = HWControls.ReadHardButton();
            System.Console.WriteLine(x);
            Thread.Sleep(20000);

        }
        void ImageRefresh(Capture scan, EventArgs e)
        {
            System.Console.WriteLine("Hello World!");
            Bitmap tempBitmap = new Bitmap(512, 512);
            ImageBuilding.Build2D(ref tempBitmap, buffer, ScanConv);
            tempBitmap.Save(@"C:\Users\koner\Desktop\img.jpg");
        }
        public int bajs = 0;
        void WatchHWButtonPressed(HWControls HwCtrl, EventArgs e)
        {
            System.Console.WriteLine("Hello World!");
            bajs += 1;
        }


        void SetDoubler()
        {
            if (ScanConverter.Doubler)
            {
                //butDoubler.BackColor = Color.OrangeRed;
                HWControls.EnableDoubler();
            }
            else
            {
                //butDoubler.BackColor = Color.LightSteelBlue;
                HWControls.DisableDoubler();

            }
        }

        void SetCompound()
        {
            if (ScanConv.Compound == true)
            {
                //butCompound.BackColor = Color.OrangeRed;
                HWControls.EnableCompound();
                //labelCompound.Visible = true;
            }
            else
            {
                //butCompound.BackColor = Color.LightSteelBlue;
                HWControls.DisableCompound();
                //labelCompound.Visible = false;
            }

        }

        private void InitScan()
        {
            if (Scan2D.ScanOn == true)
                return;

            bool bIdle = false;

            /// Initialize bmpUSImage
            Bitmap bmpUSImage = new Bitmap(512, 512, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            Bitmap bmpOut = new Bitmap(512, 512, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);//ici au lieu de refresh

            ///Initialize Doubler 
            SetDoubler();
            /// initialize Compound
            SetCompound();
            ///Send frequency to HW
            HWControls.SetFrequencyAndFocus(1, 1, 0);
            ///Send High Voltage value to HW
            HWControls.SendHighVoltage(20, 10);
            ///Enable High Voltage
            HWControls.EnableHighVoltage();
            //Send the dynamic value to HW
            HWControls.SendDynamic(30);


            ///Calculate the scanconverter according to the characteristics of the image
            int iDepth = 40;
            /*            if (false == true)
                //To set the new PRF and to update the Frame Rate consequently
                //HWControls.UpdateCfmPrf(iDepth, aiPRF[bytPrfIndex], uiNbOfLines);
                int i = 100;*/
            ScanConverter.ScanConverterError error = ScanConv.HardInitScanConverter(iDepth, 512, 512, 0, 0, ref Scan2D, ref ImageBuilding); /// Calculation of ScanConverter

            if ((error == IntersonArray.Imaging.ScanConverter.ScanConverterError.OVER_LIMITS) ||
                (error == IntersonArray.Imaging.ScanConverter.ScanConverterError.UNDER_LIMITS) ||
                (error == IntersonArray.Imaging.ScanConverter.ScanConverterError.PROBE_NOT_INITIALIZED) ||
                (error == IntersonArray.Imaging.ScanConverter.ScanConverterError.ERROR) ||
                (error == IntersonArray.Imaging.ScanConverter.ScanConverterError.WRONG_FORMAT) ||
                (error == IntersonArray.Imaging.ScanConverter.ScanConverterError.UNKNOWN_PROBE)
                )
            {
                
                //FormMessage formMessage = new FormMessage();
                //formMessage.ShowMessage((object)MessageClass.ScanConverterMessage);
                //formMessage.Dispose();
                //bInitDone = false;
            }
            else
            {

                ///Adapt the size of the form
                ResizeForm(0);

                ///Draw the depth Scale
                //uctrlDepth.BuildDrawScale(null, iDepth, this.ScanConv, bIsUpDown, fltZoomFactor, iOffsetScale);

                ///Done
                bool bInitDone = true;
            }

            void ResizeForm(int i)
            {

                this.Size = new Size(512 + 508, 512 + 256);
            }
        }
    }
}
