/*==========================================================*/
// Copyright © The Skymu Team and other contributors.
// For any inquiries or concerns, email contact@skymu.app.
/*==========================================================*/
// Modification or redistribution of this code is governed
// by the terms set out in the project license agreement.
// If you do not comply with those terms, you may not
// modify or distribute any original code from the project.
/*==========================================================*/
// License: https://skymu.app/legal/license
// SPDX-License-Identifier: AGPL-3.0-or-later
/*==========================================================*/

using System.IO;

namespace Skymu.Helpers
{
    public static class ImageHelper
    {
        public static string ResolveExtension(byte[] bytes, string existingName)
        {
            string ext = Path.GetExtension(existingName)?.ToLowerInvariant();
            if ( // does the file already have the extension? (unlikely)
                ext == ".png"
                || ext == ".jpg"
                || ext == ".jpeg"
                || ext == ".gif"
                || ext == ".webp"
            )
            {
                return existingName; // just save as is, the file has the extension already
            }

            if (bytes.Length >= 4) // are there magic bytes?
            {
                if ( // do they match up with any of the formats we know?
                    bytes[0] == 0x89
                    && bytes[1] == 0x50
                    && bytes[2] == 0x4E
                    && bytes[3] == 0x47
                )
                    return existingName + ".png"; // save as PNG
                if (bytes[0] == 0xFF && bytes[1] == 0xD8)
                    return existingName + ".jpg"; // save as JPEG
                if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
                    return existingName + ".gif"; // save as GIF
                if (
                    bytes[0] == 0x52
                    && bytes[1] == 0x49
                    && bytes[2] == 0x46
                    && bytes[3] == 0x46
                )
                    return existingName + ".webp"; // save as WebP
            }

            return existingName; // couldn't find proper extension, just save without an extension
        }
    }
}
