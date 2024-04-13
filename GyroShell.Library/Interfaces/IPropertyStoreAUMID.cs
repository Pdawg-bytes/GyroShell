﻿#region Copyright (License GPLv3)
// GyroShell - A modern, extensible, fast, and customizable shell platform.
// Copyright (C) 2022-2024  Pdawg
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static GyroShell.Library.Helpers.Win32.Win32Interop;

namespace GyroShell.Library.Interfaces
{
    public class IPropertyStoreAUMID
    {
        [ComImport]
        [Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyStore
        {
            void GetCount(out uint propCount);

            void GetAt(uint propertyIndex, out PropertyKey key);

            void GetValue(ref PropertyKey key, out PropVariant value);

            void SetValue(ref PropertyKey key, ref PropVariant value);

            void Commit();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PropertyKey
        {
            public Guid FormatId;
            public int PropertyId;

            public PropertyKey(Guid formatId, int propertyId)
            {
                FormatId = formatId;
                PropertyId = propertyId;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PropVariant : IDisposable
        {
            public ushort VarType;
            public ushort Reserved1;
            public ushort Reserved2;
            public ushort Reserved3;
            public IntPtr Data;

            public VariantUnion Value;

            public void Dispose()
            {
                PropVariantClear(ref this);
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct VariantUnion
        {
            [FieldOffset(0)]
            public sbyte I1;
            [FieldOffset(0)]
            public byte UI1;
            [FieldOffset(0)]
            public short I2;
            [FieldOffset(0)]
            public ushort UI2;
            [FieldOffset(0)]
            public int I4;
            [FieldOffset(0)]
            public uint UI4;
            [FieldOffset(0)]
            public long I8;
            [FieldOffset(0)]
            public ulong UI8;
            [FieldOffset(0)]
            public float R4;
            [FieldOffset(0)]
            public double R8;
            [FieldOffset(0)]
            public IntPtr Pointer;
            [FieldOffset(0)]
            public IntPtr PUI1;
            [FieldOffset(0)]
            public IntPtr PUI2;
            [FieldOffset(0)]
            public IntPtr PUI4;
            [FieldOffset(0)]
            public IntPtr PI8;
            [FieldOffset(0)]
            public IntPtr PUI8;
            [FieldOffset(0)]
            public IntPtr PR4;
            [FieldOffset(0)]
            public IntPtr PR8;
            [FieldOffset(0)]
            public IntPtr PBool;
            [FieldOffset(0)]
            public IntPtr PCY;
            [FieldOffset(0)]
            public IntPtr PDate;
            [FieldOffset(0)]
            public IntPtr PFileTime;
            [FieldOffset(0)]
            public IntPtr PBStr;
            [FieldOffset(0)]
            public IntPtr PUnknown;
            [FieldOffset(0)]
            public IntPtr PStream;
            [FieldOffset(0)]
            public IntPtr PIStream;
            [FieldOffset(0)]
            public IntPtr PStorage;
            [FieldOffset(0)]
            public IntPtr PIStorage;
            [FieldOffset(0)]
            public IntPtr PVersionedStream;
            [FieldOffset(0)]
            public IntPtr PClipData;
            [FieldOffset(0)]
            public IntPtr PHString;
            [FieldOffset(0)]
            public IntPtr PBitmap;
            [FieldOffset(0)]
            public IntPtr PGuid;
            [FieldOffset(0)]
            public Blob Blob;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Blob
        {
            public uint Length;
            public IntPtr Data;
        }
    }
}
