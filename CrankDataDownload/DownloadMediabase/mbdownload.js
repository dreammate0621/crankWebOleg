//debugger;

var fs = require('fs');

var mbUser = 'crank1';
var mbPassword = 'bgriffin';
var userAgentString = 'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36';

var loginUrl = 'http://www.mediabase.com/WebLogon/WebLogon.asp';
var loginPageUrl = 'http://www.musicinfosystems.com/FramesMainMenu.asp';
var stationMainPage = 'http://www.mediabase.com/mmrweb/stations/index.asp?MBHome=Y';
var allUSStationUrl = 'http://www.mediabase.com/mmrweb/stations/monitorMenuModeM.asp';
var allUSFormatPageUrl = 'http://www.mediabase.com/mmrweb/stations/Panels.asp?ob=11&format=XXM';
var allUSFormatOwnerPageUrl = 'http://www.mediabase.com/mmrweb/stations/Panels.asp?format=XXM&mode=owners';
var allUSFormatOwnerCSVUrl = 'http://www.mediabase.com/mmrweb/stations/panels.csv?format=XXM&ob=1&ChartType=M&MODE=owners';


//URLs for Charts
var sevenDayReportsMainUrl = 'http://www.mediabase.com/mmrweb/7/index.asp?MBHome=Y';
var sevenDayRollingChartsUrl = 'http://www.mediabase.com/mmrweb/7/index.asp?SiteChartMode=M';

var formatUrls =
[
    { format: 'Mainstream AC', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=a1m' },
    { format: 'Hot AC', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=a2m' },
    { format: 'Triple A', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=r1m' },
    { format: 'Active Rock', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=r2m' },
    { format: 'Alternative', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=r3m' },
    { format: 'Classic Rock', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=r4m' },
    { format: 'Adult Hits', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=a4m' },
    { format: 'Urban', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=u1m' },
    { format: 'Urban AC', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=u2m' },
    { format: 'Top 40', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=h1m' },
    { format: 'Rhythmic', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=y0m' },
    { format: 'Dance', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=y1m' },
    { format: 'Rhythmic AC', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=u4m' },
    { format: 'Country', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=c1m' },
    { format: 'Gospel', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=g1m' },
    { format: 'Oldies Traditional', url: 'http://www.mediabase.com/mmrweb/7/Charts.asp?GRC=C&FORMAT=o1m' }
];

var snapshotCounter = 0;
var downloadFolder = 'download/';

var allFormatLabel = 'All Formats';
var meidabBaseLabel = 'MEDIABASE';
var allUSFormatCSVFile = downloadFolder + '/stations/allusformat.csv';
//Include utils from CasperJS lib

var utils = require('utils');

//Create CasperJS object with custom viewport, useragent

var casper = require('casper').create(
{
    viewportSize:
    {
        width: 1600,
        height: 1050
    },
    userAgent: userAgentString,
    waitTimeout: 60000,
    //logLevel: "debug",
    verbose: true
});


//Register for page.error events - Capture page related events
casper.on('page.error', function (err)
{
    this.echo('page.error: ' + JSON.stringify(err));
}
);



//var chartModes = [];
//var formats = [];

var printTitleUrl = function (sshot)
{
    this.echo(this.getTitle());
    this.echo(this.getCurrentUrl());

    var capture = typeof sshot !== 'undefined' ? sshot : true;
    if (capture)
    {
        snapshotCounter++;
        this.capture('sshots/' + snapshotCounter + '.png');
    }
};

var terminate = function ()
{
    this.echo('Scraping done...shutting down...');
    this.exit();
};

var login = function ()
{
    this.echo('Login page loaded...');
    printTitleUrl.call(this);

    this.echo('After print TitleUrl');
    this.fill('form#Form1',
        {
            'userName': mbUser,
            'password': mbPassword
        }, true
    );

    //Wait for the next frame
    this.waitForUrl(loginPageUrl, function ()
    {
        printTitleUrl.call(this);

        //Click on "MEDIBASE" to goto MEDIABASE page
        this.withFrame("MISMain", function ()
        {
            this.clickLabel('MEDIABASE', 'a');
            this.then(function ()
            {
                this.echo("After clicking mediabase link");
                printTitleUrl.call(this);
            });
        });
    });
};


var loadAllFormatsPage = function ()
{
    this.thenOpen(stationMainPage, function ()
    {
        this.echo("After clicking station link");
        printTitleUrl.call(this);

        chartModes = this.evaluate(getChartModes);

        this.echo("Chart modes found...");
        utils.dump(chartModes);

        //Select "Mediabase - All Stations (U.S.)"
        this.fill('form',
            {
                'SiteChartMode': 'M'
            }, true
        );

        this.waitForSelector('select[name="SiteChartMode"]', function ()
        {
            printTitleUrl.call(this);
            this.echo("Inside All US Station page");
            //Click "All Formats"
            this.echo('Clicking on "All Formats" link...');
            this.clickLabel(allFormatLabel, 'a');

            this.waitForUrl(allUSFormatPageUrl, function ()
            {
                this.echo("Inside All US stations All Formats page");
                printTitleUrl.call(this, false);

                //Enable show owners options
                this.click("a[href='/mmrweb/stations/Panels.asp?format=XXM&mode=owners']");
                this.waitForUrl(allUSFormatOwnerPageUrl, function ()
                {
                    this.echo("Inside All US Staions with Owner page");
                    printTitleUrl.call(this, false);

                    this.echo("Downloading All Format Station data CSV file..");

                    this.download(allUSFormatOwnerCSVUrl, allUSFormatCSVFile);
                });
            });
        });
    });
};


var getChartModes = function ()
{
    //Get ChartMode
    var chartModeSelect = document.querySelector('select[name="SiteChartMode"]');
    var charts = [];
    console.log(chartModeSelect);
    for (i = 0; i < chartModeSelect.options.length; i++)
    {
        var item = chartModeSelect.options[i];
        console.log("Option value = " + item.value + "; text=" + item.text);
        charts.push(item.text);
    }
    return charts;

};

var loadAllCharts = function ()
{
    this.thenOpen(sevenDayReportsMainUrl, function ()
    {
        this.echo("After clicking 7 Days link");
        printTitleUrl.call(this);

        //Select "Mediabase - All Stations (U.S.)"
        this.fill('form',
            {
                'SiteChartMode': 'M'
            }, true
        );

        this.echo("After posting to All US stations");

        this.waitForSelector('select[name="SiteChartMode"]', function ()
        {
            this.echo("Inside All US Stations chart");
            printTitleUrl.call(this);

            this.each(formatUrls, function (self, formatUrlObj)
            {
                var format = formatUrlObj.format;
                var formatUrl = formatUrlObj.url;

                this.echo("Format = " + format + ", URL= " + formatUrl);

                this.thenOpen(formatUrl);

                this.waitForSelector('select[name="FORMAT"]', function ()
                {
                    this.echo("Format = " + format + ", URL= " + formatUrl);
                    printTitleUrl.call(this);
                    this.echo("Inside 7 days chart");
                    var currentFormat = this.fetchText('div.Title');
                    this.echo("Current format is " + currentFormat);

                    //Find URL for Donwload to Spreadsheet
                    var spreadSheetLink = this.getElementAttribute('a[href*="/mmrweb/7/charts.csv?format="]', 'href');

                    this.echo("The link is " + spreadSheetLink);

                    this.download(spreadSheetLink, downloadFolder +"/songs/" + format + ".csv");
                    this.echo("Download is complete!");
                });
            });
        });
    });
};

casper.echo("Starting station scraping....");
casper.start(loginUrl);

casper.page.onConsoleMessage = function (msg)
{
    console.log(msg);
};


casper.page.onError = function (msg, trace)
{
    var msgStack = ['PHANTOM ERROR: ' + msg];
    if (trace && trace.length)
    {
        msgStack.push('TRACE:');
        trace.forEach(function (t)
        {
            msgStack.push(' -> ' + (t.file || t.sourceURL) + ': ' + t.line + (t.function ? ' (in function ' + t.function + ')' : ''));
        });
    }
    console.error(msgStack.join('\n'));
    phantom.exit(1);
};

casper.on('error', function (msg, trace)
{
    var msgStack = ['PHANTOM ERROR: ' + msg];
    if (trace && trace.length)
    {
        msgStack.push('TRACE:');
        trace.forEach(function (t)
        {
            msgStack.push(' -> ' + (t.file || t.sourceURL) + ': ' + t.line + (t.function ? ' (in function ' + t.function + ')' : ''));
        });
    }
    console.error(msgStack.join('\n'));
});

casper.echo("After login.....");

casper.then(login);
//Load all Formats page
casper.then(loadAllFormatsPage);

//Load all Charts
casper.then(loadAllCharts);

casper.run(terminate);

console.log("Finished...");


//casper.page.onConsoleMessage = function (msg)
//{
//    console.log(msg);
//};