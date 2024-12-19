<img src="https://github.com/mrklintscher/libfintx/blob/master/res/logo.png" align="right">

# libfintx

[![Build Status](https://travis-ci.org/libfintx/libfintx.svg?branch=master)](https://travis-ci.org/libfintx/libfintx)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/928e912657d44a6090d329343aa13346)](https://www.codacy.com/app/torsten-klinger/libfintx?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=mrklintscher/libfintx&amp;utm_campaign=badger)
[![Issue Count](https://codeclimate.com/github/libfintx/libfintx/badges/issue_count.svg)](https://codeclimate.com/github/libfintx/libfintx)

An C# based client library for **HBCI 2.2**, **FinTS 3.0**, **EBICS H004** and **EBICS H005**.

In 1995 the ZKA announced a common online banking standard called *Homebanking Computer Interface* (HBCI). In 2003 they published the next generation of this protocol standard and named it *Financial Transaction Services* (FinTS).

Today most of all german banks support this online banking standards.

The Electronic Banking Internet Communication Standard (EBICS) is a German transmission protocol developed by the German Banking Industry Committee for sending payment information between banks over the Internet. It grew out of the earlier BCS-FTAM protocol that was developed in 1995, with the aim of being able to use internet connections and TCP/IP. It is mandated for use by German banks and has also been adopted by France and Switzerland. [Wikipedia](https://en.wikipedia.org/wiki/Electronic_Banking_Internet_Communication_Standard).

This client library supports all four APIs, HBCI 2.2, FinTS 3.0 and EBICS H004 and H005.

It can be used to read the balance of a bank account, receive an account statement, and make a SEPA payment using **PIN/TAN** and **EBICS**.

# Usage

There are many reasons why you need to use a banking library which can exchange data from your application with the bank. One reason for example is to found a [Fintech](https://de.wikipedia.org/wiki/Finanztechnologie).

# Target platforms

* .NET Standard 2.0

# Sample

Look at the demo projects inside the master branch.

# Features

* Get Balance (**HKSAL**)
* Request Transactions (**HKKAZ**)
* Transfer money (**HKCCS**)
* Transfer money at a certain time (**HKCCS**)
* Collective transfer money (**HKCCM**)
* Collective transfer money terminated (**HKCME**)
* Rebook money from one to another account (**HKCUM**)
* Collect money (**HKDSE**)
* Collective collect money (**HKDME**)
* Load mobile phone prepaid card (**HKPPD**)
* Submit banker's order (**HKCDE**)
* Get banker's orders (**HKCSB**)
* Send Credit Transfer Initiation (**CCT**)
* Send Direct Debit Initiation (**CDD**)
* Pick up Swift daily statements (**STA**)

# Specification

For exact information please refer to the [german version of the specification](http://www.hbci-zka.de/spec/spezifikation.htm).

# Tested banks

* Raiffeisenbanken
* Sparkassen
* DKB
* DiBa
* Consorsbank
* Sparda
* Postbank
* Norisbank
* Deutsche Bank
* Unicredit Bank
* Commerzbank

# Sample code

Check account balance.

```csharp
/// <summary>
/// Kontostand abfragen
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private async void btn_kontostand_abfragen_Click(object sender, EventArgs e)
{
    var connectionDetails = GetConnectionDetails();
    var client = new FinTsClient(connectionDetails);
    var sync = await client.Synchronization();

    HBCIOutput(sync.Messages);

    if (sync.IsSuccess)
    {
        // TAN-Verfahren
        client.HIRMS = txt_tanverfahren.Text;

        if (!await InitTANMedium(client))
            return;

        var balance = await client.Balance(_tanDialog);

        HBCIOutput(balance.Messages);

        if (balance.IsSuccess)
            SimpleOutput("Kontostand: " + Convert.ToString(balance.Data.Balance));
    }
}
```

# SSL verification

The verification process is done by using the default [**WebRequest**](https://msdn.microsoft.com/de-de/library/system.net.webrequest(v=vs.110).aspx) class.

# Limitations

* Only PIN/TAN security procedure is supported when using HBCI
* Usage with certificates has been prepared but not completely implemented yet. It works with private/public keys.
* Only version A005 for signatures can be used. A006 uses PSS padding, which is currently not supported by .NET Core 2.x. Bouncy Castle is only used for PEM file and certificate management.
* Only version E002 for encryption can be used.
* Only version X002 for authentication can be used.
* It was developed using EBICS Version H004, but H005 should work.

# Copyright & License

Copyright (c) 2016 - 2021 **Torsten Klinger**

Licensed under **GNU LESSER GENERAL PUBLIC LICENSE Version 3, 29 June 2007**. Please read the LICENSE file.

# Support

You can contact me via [E-Mail](mailto:torsten.klinger@googlemail.com).

# Community

Join our developer community at [Slack](https://join.slack.com/t/libfintx/shared_invite/zt-qut8s3qo-r8uCXey6NZIo_swwtHhvzw) to discuss bugs and further developments with us.
