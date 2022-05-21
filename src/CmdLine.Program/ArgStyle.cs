// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

namespace ConsoleFx.CmdLine
{
    public enum ArgStyle
    {
        // As per https://www.gnu.org/prep/standards/html_node/Command_002dLine-Interfaces.html, this
        // is not a POSIX style. POSIX only uses single char (single dash options).
        // POSIX usage guide: https://pubs.opengroup.org/onlinepubs/9699919799/basedefs/V1_chap12.html
        GnuGetOpts,

        Windows,
    }
}
