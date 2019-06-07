## Unix style command line args
* Supports long (`--long`) and short (`-s`) style options.
* Short options that do not accept parameters can be combined.
    ```sh
    > copy file1.txt destdir -f -v -y
    
    # can be written as
    > copy file1.txt destdir -fvy
    ```
* Supports rest args, where all args specified after a `--` are considered arguments, even ones that look like options.
    ```sh
    # Here, everything after the -- is considered an argument,
    # even --force, --verbose and -y
    > npm run script -- copy file1.txt destdir --force --verbose -y
    ```
    