# Kerkesix event tracker

This project is a dedicated event tracker solution for Kerkesix [walking event](http://kerkesix.fi/sysimusta-satku). Hosted version is (or used to be) available at: [http://satku.kerkesix.fi](http://satku.kerkesix.fi).

Project has been a technology testing platform for [Tero Teelahti](http://teelahti.fi)
for over ten years. It has always had bleeding edge technology that has changed from
year to year. The final version you see here is a simplified archive version that is
easier to run locally on any platform, and does not need as many external dependencies.
If this event would have continued, a rewrite to React + Redux would have been due (as
it would fit perfectly on the event based approach at server side).

## History

### v1, ~2006

First version had a separate command line client that outputted bar code readings to a CSV
file, which was then imported to the web site through UI. This of course did not handle
any real time edge cases that well. Technology was ASP.NET WebForms and Canvas (which
was very new at that time). Site was hosted at Kerkesix current web hosting, and database
was MS SQL.

Admin console was super simple and used naive username/password login.

Analytics was done with Google Analytics.

### v2, ~2008

Second version was a rewrite to ASP.NET MVC, which was not even released at that time. This
version also changed database to Raven.NET, which was also brand new alpha-level document
database at that time.

### v3, ~2009

Third version was a rewrite to be hosted at Azure Web Sites, with Azure Table Storage as
backend. At this time storage was used as a document storage. one row per one attendee. This
was a must as Raven.NET was not easily hosted on Azure at that time.

This version lasted for a while, and was extended with registration, automatic email sending
from the site etc.

Admin console used Google, Facebook and Microsoft account authentication.

This was the first version that had proper deployment pipeline with separate DEV, QA, and PROD
staging areas. Deployments were automated during the years before the next rewrite.

### v4, ~2012

Fourth version was the pinnacle of the whole site: it was a rewrite to Command Query Segregation Principle (CQRS) style application, using event sourcing as data storage style. This meant, that there was no state as is in the backend, just events that created the state when they were replayed on site start. Another very fancy change was the real time functionality: all users
that had the site open shared the same state via WebSockets (or fallbacks via SignalR). This
included client side persistent buffer queues for bad networks, building the state in two
different ways in client and in server, and all other complexities.

This rewrite also introduced persistent pub/sub queues, that were used to gain transactionality
behavior with non-transactional resources. Each incoming command would go through a complex set
of cloud queue topics with retries, duplicate checking and the whole lot. At this time the site would have scaled to very high simultaneous users, which was naturally completely unnecessary as
this site has max couple of hundred simultaneous users.

Finally, this version was a Knockout single page application, with all kinds of dynamic view
loading capabilities. TypeScript was brought into play.

Azure's Application Insights was introduced as second analytics platform.

Self service registration was added.

During this versions lifetime MVC and Knockout versions were changed, and a separate command line tool
was introduced as operating and debugging pure event data proved to be very hard.
CLI tool also had features to upload old event's data from old format into the new.

### v5, 2017

The last version was a rewrite to .NET Core and ASP.NET Core to get proper cross platform support.
As very many third party dependencies would not work Core, the site was dramatically simplified.
Lots of stuff remained unchanged still, like the way Javascript is loaded, and the client side
SPA framework. Next versions would have introduced WebPack loading and newer SPA framework.

In this release registration was also separated to be a separate site.

At the same time organizing the event had lost some of its glory and the future of the event is
uncertain. Therefore the repository was cleaned up and archived to Github to
public - unfortunately with all the history removed to keep the Git repo size in control and to
protect the attendees as some versions had uploadable attendee data inside the repo.