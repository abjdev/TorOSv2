# EtorumOS
EtorumOS is a simple C# CLI OS using COSMOS

## How do I use EtorumOS?
As of now, there aren't any public builds. To use EtorumOS, please install the COSMOS DevKit and then build the project yourself.
#### Warning: This OS, and any CosmosOS in general are not safe to use on real machines. Use a VM.

## EtorumOS basics
EtorumOS uses a CLI to accept commands. It also has an user account system (UAS). The CLI interprets a very basic language called ETOScript - it only features string parsing and executing (multiple) commands and this will probably stay like that. Commands that you can are, for example, `cd, ls, read, setpassword, cedit` and more!



#### CEdit (CommandEdit)
CEdit is an editor that is controlled by using commands. And I am gonna be honest, I just failed at implementing a proper editor lol  
CEdit has a `help` command that shows you all commands available. CEdit itself also interpret ETOScript but it **only** includes CEdit commands.

#### Path limitations
As of now, only navigating using `cd` supports the .. "operator" to go back a folder. A proper path parsing implementation will be added soon.

#### User folder & autorun.etos
After logging in for the first time, you will be in your user folder by default. (`0:\users\<username\`)  
This folder only has one default file called autorun.etos, this file can contain any ETOScript that will execute as soon as you log in.

#### UserAccountService (UAS)
The UAS (UserAccountService) is currently very unsafe - passwords are stored in plaintext and can be read by any user. Adding new users is currently also only possible by using `cedit`
