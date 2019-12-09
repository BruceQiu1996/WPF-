using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using LINQPad;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SearchAnyWay
{
    [POCOViewModel]
    public class MainViewModel : ViewModelBase
    {
        //加载中
        private bool _loading;
        public bool IsLoading
        {
            get { return this._loading; }
            set
            {
                SetProperty(ref _loading, value, () => IsLoading);
            }
        }
        //贴吧名
        private string _hub;
        public string HubName
        {
            get { return this._hub; }
            set
            {
                SetProperty(ref _hub, value, () => HubName);
            }
        }
        //爬取页数
        private int _page;
        public int Page
        {
            get { return this._page; }
            set
            {
                SetProperty(ref _page, value, () => Page);
            }
        }
        //帖子集合
        public ObservableCollection<ArticleModel> _source;
        public ObservableCollection<ArticleModel> Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value, ()=>Source); }
        }

        public AsyncCommand SearchCommand { get; set; }

        public IEnumerable<int> PageRange { get; private set; }

        public MainViewModel()
        {
            Page = 10;
            PageRange = new List<int>() { 10,50, 100, 200, 500, 1000, 10000 };
            Source = new ObservableCollection<ArticleModel>();
            SearchCommand = new AsyncCommand(Search);
        }
        private async Task Search()
        {
            if (string.IsNullOrEmpty(HubName?.Trim()))
                return;
            IsLoading = true;
            try
            {
                await Task.Run(() =>
                {
                    var http = new HttpClient();
                    var parser = new HtmlParser();
                    var result=Enumerable.Range(0, Page)
                        .AsParallel()
                        .AsOrdered()
                        .SelectMany(page =>
                        {
                            return Task.Run(async () =>
                            {
                                var pnIndex = page * 50;
                                //获取请求后response的页面代码。
                                string pageData = await http.GetStringAsync($"https://tieba.baidu.com/f?kw={HubName}&ie=utf-8&pn={pnIndex}".Dump());
                                //AngleSharp解析页面代码
                                IHtmlDocument doc = await parser.ParseDocumentAsync(pageData);
                                return doc.QuerySelectorAll(".t_con.cleafix").Select(tag => new ArticleModel()
                                {
                                    Title = tag.QuerySelector(".j_th_tit").TextContent?.Trim(),
                                    Brief= tag.QuerySelector(".threadlist_abs.threadlist_abs_onlyline")?.TextContent?.Trim(),
                                    CommentCount=Convert.ToInt32(tag.QuerySelector(".threadlist_rep_num.center_text")?.TextContent),
                                    AuthorName=tag.QuerySelector(".frs-author-name.j_user_card")?.TextContent?.Trim(),
                                }); ;
                            }).GetAwaiter().GetResult();
                        });
                    Source = new ObservableCollection<ArticleModel>(result);
                });
            }
            catch { }
            finally
            {
                IsLoading = false;
            }
        }
    }
    public class ArticleModel
    {
        public string Title { get; set; }
        public string Brief { get; set; }
        public int CommentCount { get; set; }
        public string AuthorName { get; set; }
    }
}
