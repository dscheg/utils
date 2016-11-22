use std::env;

use std::io;
use std::io::prelude::*;

extern crate getopts;
use getopts::Options;

extern crate rustc_serialize;
use rustc_serialize::json::Json;

macro_rules! println_stderr(
    ($($arg:tt)*) => ({
        writeln!(&mut io::stderr(), $($arg)*).expect("Failed to print to stderr");
    })
);

fn print_usage(program: &str, opts: Options) {
    let brief = format!("Usage: {} [OPTION]... FIELD1[,FIELD2[,PATH/TO/FIELD3...]]", program);
    print!("{}", opts.usage(&brief));
}

fn main() {
    let args: Vec<String> = env::args().collect();

    let mut opts = Options::new();
    opts.optopt("d", "delim", "use DELIM instead of TAB for field delimiter", "DELIM");
    opts.optflag("?", "help", "print this help");

    let matches = match opts.parse(&args[1..]) {
        Ok(m) => m,
        Err(e) => {
            println_stderr!("Error: {}", e.to_string());
            return;
        }
    };

    if matches.opt_present("?") || matches.free.is_empty() {
        print_usage(&args[0], opts);
        return;
    }

    let delim = matches.opt_str("d").unwrap_or("\t".to_string());
    let fields = matches.free[0].split(',').map(|field| field.split('/').collect::<Vec<_>>()).collect::<Vec<_>>();

    let len = fields.len();

    let stdin = io::stdin();
    for (num, line) in stdin.lock().lines().enumerate() {
        let json = match Json::from_str(&line.unwrap_or(String::with_capacity(0))) {
            Ok(v) => v,
            Err(e) => {
                println_stderr!("Error at line {}: {}", num + 1, e);
                continue;
            }
        };
        let mut values: Vec<String> = Vec::with_capacity(len);
        for field in &fields {
            values.push(match json.find_path(&field) {
                Some(value) => match value.as_string() { None => value.to_string(), Some(v) => v.to_string() },
                None => String::with_capacity(0)
            });
        }
        println!("{}", values.join(&*delim));
    }
}
