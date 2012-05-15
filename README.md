<h1>wfSpy</h1>

<p>wfSpy is the open source Windows Forms spying utility created way back in 2003 by <a href="http://www.codeproject.com/script/Articles/MemberArticles.aspx?amid=15383">Rama Krishna Vavilala</a>. <a href="http://www.codeproject.com/Articles/4814/A-simple-Windows-forms-properties-spy">Here</a> is the original CodeProject article.</p>

<p>It allows you to spy/browse the window hierarchy of a running Windows Forms application (without the need for a debugger) … and to change properties as well.</p>

I work for <a href="http://www.starkey.com/">Starkey Laboratories, and in my work there, we modified and enhanced wfSpy (and continue to do so) for our purposes. I have an <a href="http://www.cplotts.com/2009/10/28/an-ode-to-wfspy/">article</a> at my blog which goes into the few, but nice, features that we added.

<h2>.NET 1.1, 2.0, 4.0 Versions</h2>

<p>I have checked in (and tagged) three different versions ... each corresponding to the version of .NET CLR that it works with. That is, if you have a .NET 1.1 application that hasn't been converted to a more recent version of the .NET Framework ... you will need to get and build that version of the utility.</p>

<p>The master branch currently is the .NET 4.0 version of the utility ... i.e. the latest released version of the framework as of this writing. I speculate that wfSpy will need to be modified yet again when .NET 4.5 releases since there is a new version of the CLR coming with .NET 4.5.</p>

<h2>x86 Only</h2>

<p>I currently have not gone to any effort to get wfSpy to work with 64-bit applications. It shouldn't be too hard, though. If you do get it to work with 64-bit applications, please contribute those changes back to this project.</p>
