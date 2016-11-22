use std::process::Command;
use std::env;

fn main() {
    let out_dir = env::var("OUT_DIR").ok().expect("can't find out_dir");

    Command::new("C:\\Program Files (x86)\\Windows Kits\\8.1\\bin\\x64\\rc.exe")
        .arg("/nologo")
        .arg("/v")
        .arg("/fo").arg(&format!("{}/main.res.lib", out_dir))
        .arg("src/main.rc")
        .status()
        .unwrap();

    println!("cargo:rustc-link-search=native={}", out_dir);
    println!("cargo:rustc-link-lib=static=main.res");
}
