# pWallet 2.0
This is a UI for Phoenixd Server by ACINQ.

Its built in .NET 9 Blazor as a web application with PWA support so its "installable" through browser on your phone by adding it to home screen but can be used on a laptop through browser ( even here installable because of PWA support ).

You can also if you host or handle your DNS to your domain add cloudflare api credentials to add custom BIP353 address to your phoenixd server.

As for now there is only one built version in the repo and that is for Windows users.
But since its .NET you can build the project and run it on any OS.

It works with a SQLite database for account management and also to save your cloudflare credentials.
Your Phoenixd Server credentials is added in appsettings.json

***As for now the application restricts user registrations to only ONE user***

The UI with Phoenix Server can handle:
- Lightning invoices (pay and receive)
- LNURL (pay and receive, LNURL string makes static QR to receive sats whenever)
- LNURL-P (receive at zap@domain.com)
- Lightning Address (pay and receive, see LNURL-P)
- Bolt12 offer (pay and receive)
- Bip353 address (pay and receive)
- LNAuth
- Onchain (pay)

Extra:
- It shows Sats, Btc, USD, EUR by clicking your current amount on home screen

If you have a domain and handles dns for it through Cloudflare you can also:
- Create custom Bip353 address with your domain
- Use the domain for LNURL string and LNURL-P addresses (zap@domain.com is activated and working).

**BEFORE YOU START USING THE APPLICATION**
1. Chose how to run! .exe for windows in Windows build, .dll for Linux ( needs ASP.NET Core Runtime for .NET 9 on your system ), and Dockerfile!
2. Fill the appsettings.json with your credentials, if you run through Docker, make sure you edit appsettings.json before building image.
3. Go to wwwroot/.well-known/lnurlp and copy the example file, open the copy and edit the domain, then name the file whatever you wanna use as a lightningaddress.
   If you want another lnurl-p address, just copy the example file again and name it and fix the domain in it.
   Then restart the application!
   If you run through Docker, you need to add files in Dockercontainer ( /wwwroot/.well-known/lnurlp )

<img src="https://github.com/user-attachments/assets/4440df4f-5aa0-4efb-898f-ad9ccef15a3a" width="200px"/>
<img src="https://github.com/user-attachments/assets/5e38fe40-2f44-491a-afc9-e80d1bb1e161" width="200px"/>
<img src="https://github.com/user-attachments/assets/46bc03a6-42c1-493e-8008-37e0cb23e837" width="200px"/>
<img src="https://github.com/user-attachments/assets/0ab298fe-612e-4ed2-87f5-783c95444299" width="200px"/>
<img src="https://github.com/user-attachments/assets/4091ceda-c835-4fcc-aadc-f440b595fd31" width="200px"/>
<img src="https://github.com/user-attachments/assets/0552288f-f732-496d-b900-bfffde0c256a" width="200px"/>
<img src="https://github.com/user-attachments/assets/eceab3c2-950d-4718-92c2-90199207978c" width="200px"/>
<img src="https://github.com/user-attachments/assets/b212ca46-77f9-48e1-a0fb-382c21c7e900" width="200px"/>
<img src="https://github.com/user-attachments/assets/aded9035-a8ed-4676-92fe-b8043f487b9a" width="200px"/>
<img src="https://github.com/user-attachments/assets/21f5a551-94f9-4ce2-bb28-2a48d8084caf" width="200px"/>

