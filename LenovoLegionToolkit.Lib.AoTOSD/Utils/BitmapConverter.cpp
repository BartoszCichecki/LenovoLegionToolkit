#include"BitmapConverter.h"

using namespace System;

namespace Utils = LenovoLegionToolkit::Lib::AoTOSD::Utils;

Gdiplus::Bitmap* Utils::BitmapConverter::Convert(Drawing::Bitmap^ mBmp)
{
    Drawing::Imaging::BitmapData^ bmpData = nullptr;
    Gdiplus::Bitmap* oBmp = nullptr;
    try {
        Drawing::Rectangle rect = Drawing::Rectangle(0, 0, mBmp->Width, mBmp->Height);
        bmpData = mBmp->LockBits(rect, Drawing::Imaging::ImageLockMode::ReadOnly, Drawing::Imaging::PixelFormat::Format32bppArgb);
        IntPtr ptr = bmpData->Scan0;
        oBmp = new Gdiplus::Bitmap(mBmp->Width, mBmp->Height, PixelFormat32bppARGB);
        for (int y = 0; y < mBmp->Height; y++)
        {
            Gdiplus::Color* pixel = reinterpret_cast<Gdiplus::Color*>((BYTE*)ptr.ToPointer() + y * bmpData->Stride);
            for (int x = 0; x < mBmp->Width; x++)
            {
                Gdiplus::Color color(pixel[x].GetValue());
                oBmp->SetPixel(x, y, color);
            }
        }
    }
    finally {
        if (bmpData != nullptr)
        {
            mBmp->UnlockBits(bmpData);
        }
    }
    return oBmp;
}
