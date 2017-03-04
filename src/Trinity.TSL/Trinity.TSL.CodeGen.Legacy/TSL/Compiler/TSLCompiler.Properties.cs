﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    public partial class TSLCompiler
    {
        const string src_struct_accessor = "Structs.cs";
        const string src_cell_accessor = "CellAccessor.cs";
        const string src_protocol = "Protocol.cs";
        const string src_storage = "Storage.cs";
        const string src_external = "External.cs";
        const string src_tql = "TQL.cs";
        const string phase1_assembly_name = "Trinity.Extension.Accessor";
        const string phase2_assembly_name = "Trinity.Extension.Storage";
        const string phase1_assembly = "Trinity.Extension.Accessor.dll";
        const string phase2_assembly = "Trinity.Extension.Storage.dll";

        /// <summary>
        /// Value = "Trinity.Extension.dll"
        /// </summary>
        const string default_assembly_name = "Trinity.Extension";

        #region Feature toggles and constants
        public static bool CompileWithDebugFeatures = false;
        public static bool TSLReorderOpen = false;
        internal readonly static byte[] TrinityKey = new byte[]
        {
            7, 2, 0, 0, 0, 36, 0, 0, 82, 83, 65, 50, 0, 4, 0, 0, 1, 0, 1, 0, 107, 87, 92, 88, 198, 131, 241, 209, 4, 227, 135, 25, 103, 35, 217, 94, 236, 207, 224, 181, 64, 132, 143, 226, 169, 146, 115, 157, 54, 0, 70, 248, 233, 199, 38, 246, 179, 156, 146, 139, 175, 17, 181, 168, 105, 187, 5, 68, 125, 118, 169, 36, 38, 30, 89, 59, 194, 229, 224, 27, 210, 128, 182, 120, 77, 105, 212, 201, 148, 149, 191, 202, 183, 111, 180, 204, 59, 125, 30, 183, 132, 106, 99, 206, 254, 126, 144, 52, 171, 199, 222, 22, 254, 23, 83, 109, 240, 130, 208, 222, 67, 121, 23, 252, 102, 128, 154, 158, 198, 150, 112, 34, 211, 124, 58, 115, 107, 100, 163, 249, 56, 214, 96, 63, 164, 89, 232, 191, 245, 232, 102, 233, 229, 252, 171, 143, 95, 212, 110, 163, 153, 219, 41, 45, 12, 1, 228, 30, 72, 215, 121, 197, 204, 110, 208, 199, 216, 201, 230, 255, 206, 38, 17, 86, 199, 237, 227, 83, 216, 244, 115, 164, 159, 82, 152, 146, 124, 28, 159, 112, 53, 48, 124, 157, 124, 126, 7, 192, 85, 222, 110, 242, 223, 130, 223, 120, 90, 23, 165, 144, 226, 92, 39, 50, 213, 220, 167, 76, 55, 235, 44, 106, 116, 243, 201, 151, 75, 170, 109, 70, 160, 222, 223, 206, 70, 177, 49, 153, 157, 188, 54, 14, 123, 69, 173, 58, 175, 6, 226, 249, 118, 75, 237, 214, 216, 27, 62, 175, 116, 109, 224, 221, 132, 165, 165, 202, 241, 78, 177, 10, 152, 94, 116, 117, 110, 208, 42, 81, 118, 136, 57, 99, 82, 13, 10, 99, 116, 68, 68, 8, 133, 78, 151, 47, 165, 136, 213, 73, 122, 11, 32, 20, 23, 240, 19, 217, 242, 239, 35, 119, 48, 149, 160, 64, 250, 51, 210, 105, 51, 177, 25, 31, 79, 175, 154, 142, 19, 79, 44, 40, 157, 199, 127, 143, 231, 181, 57, 119, 158, 117, 203, 248, 30, 198, 72, 240, 174, 211, 25, 48, 215, 62, 223, 62, 84, 235, 188, 180, 103, 237, 111, 194, 121, 229, 53, 106, 238, 150, 150, 42, 242, 149, 81, 98, 13, 145, 109, 242, 210, 208, 241, 168, 247, 143, 173, 173, 4, 143, 66, 166, 142, 14, 116, 169, 168, 187, 38, 7, 100, 114, 92, 8, 51, 24, 28, 17, 189, 186, 81, 191, 83, 188, 51, 103, 35, 168, 192, 175, 149, 52, 57, 221, 45, 124, 164, 125, 190, 24, 142, 108, 147, 168, 91, 148, 83, 99, 108, 24, 142, 48, 49, 124, 21, 232, 14, 67, 129, 100, 5, 130, 60, 130, 183, 78, 193, 9, 22, 214, 49, 100, 98, 243, 61, 50, 138, 94, 17, 40, 38, 93, 53, 85, 133, 81, 22, 201, 192, 221, 247, 11, 124, 151, 24, 126, 2, 132, 21, 17, 196, 126, 174, 10, 117, 152, 79, 40, 167, 239, 212, 119, 72, 69, 245, 171, 140, 199, 233, 4, 103, 126, 70, 196, 241, 60, 93, 77, 12, 202, 44, 31, 48, 161, 29, 76, 32, 233, 9, 192, 100, 32, 121, 153, 34, 75, 249, 90, 8, 221, 164, 199, 232, 5, 210, 12, 94, 57, 144, 46, 10, 18, 50, 114, 15, 115, 108, 148, 182, 193, 75, 107, 15, 148, 115, 124, 203, 29, 181, 1, 7, 121, 93, 164, 0, 8, 59, 56, 125, 126, 68, 65, 212, 12, 97, 41, 212, 167
        };
        #endregion

        static List<string> included_src_files = new List<string>();

        internal static Random tmpVarNameRandom = new Random();

        const string charset = "0123456789_qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
        const string TSLExtensionSuffix = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_";
    }
}
