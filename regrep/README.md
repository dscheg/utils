# regrep

regrep is a small search and replace command line tool

## Features
* Regular expressions support
* Encodings support
* Multithreading
* Coloring

## Usage
```
Usage: regrep [OPTIONS] PATTERN [REPLACEMENT]

Search and replace for PATTERN in standard input using regular expression.

Options:
  -e, --encoding=ENCODING    Input/output ENCODING (default UTF-8)
  -i, --ignore-case          Ignore case distinctions
  -v, --invert-match         Select non matching lines
  -o, --only-matching        Show only the part of a line matching PATTERN
  -n, --threads=VALUE        Number of threads (default 1)
  -s, --silent               Silent mode
  -h, -?, --help             Show this message

Examples:
  regrep \d
  regrep \s+ " "
  regrep -e1251 [Пп]ривет
  regrep -eUTF-16LE \x22\p{Lu}+\x22
  regrep -o "\b(\d{2})\.(\d{2})\.(\d{4})\b" "$3-$2-$1\t$'"

Substitutions:
  $$       Single $ literal
  $NUMBER  Last substring matched by the capturing group identified by NUMBER
  ${NAME}  Last substring matched by the named group identified by (?<NAME>)
  $&       Copy of the entire match
  $`       Text of the input string before the match
  $'       Text of the input string after the match
  $+       Last group captured
  $_       Entire input string

Supports character escapes in both PATTERN and REPLACEMENT:
  \X       Well-known \a, \b, \e, \t, \r, \n, \v, \f
  \cX      ASCII control char, X is the letter of the control char
  \NNN     Two or three digits octal character code
  \xNN     Two-digit hexadecimal character code
  \uNNNN   UTF-16 hexadecimal code unit

Exit status is 0 if any line is selected, 1 otherwise, 2 if any error occurs.
```
