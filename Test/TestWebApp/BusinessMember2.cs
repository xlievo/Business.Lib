﻿using Business;
using Business.Attributes;
using Business.Result;
using Business.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static Args;
using static Startup;
using Business.Data;
using LinqToDB;
using LinqToDB.Linq;
using LinqToDB.Extensions;
using LinqToDB.Expressions;
using LinqToDB.Mapping;

[JsonArg]
[Logger]
[Info("API")]
public class BusinessMember2 : BusinessBase
{
    static BusinessMember2()
    {
        var con = Startup.appSettings.GetSection("Redis").GetSection("ConnectionString").Value;
        System.Console.WriteLine($"Redis={con}");
        var csredis = new CSRedis.CSRedisClient(con);
        RedisHelper.Initialization(csredis);

        RedisHelper.HSetAsync("Role", "value", "111");
        RedisHelper.HSetAsync("Role", "value2", "222");
        RedisHelper.HSetAsync("Role", "value3", "333");

        var values = RedisHelper.HGetAll("Role");
    }

    public BusinessMember2()
    {
        this.Logger = x =>
        {
            using (var con = Context.DataBase.DB.GetConnection())
            {
                con.Save(new DataModel.dd { dd2 = "111", dd_Column = "222" });
            }

            try
            {
                System.Threading.SpinWait.SpinUntil(() => false, 3000);

                x.Value = x.Value?.ToValue();

                var log = x.JsonSerialize();

                Help.WriteLocal(log, console: true, write: x.Type == LoggerType.Exception);
            }
            catch (Exception exception)
            {
                Help.ExceptionWrite(exception, true, true);
            }
        };
    }

    public virtual async Task<dynamic> Test001(Business.Auth.Token token, Arg<Test001> arg, [Ignore(IgnoreMode.BusinessArg)][Test2]decimal mm = 0.0234m)
    {
        Context.DataBase.DB.Save(new DataModel.dd { dd2 = "111", dd_Column = "222" });

        using (var con = Context.DataBase.DB.GetConnection())
        {
            con.BeginTransaction();

            con.Save(new DataModel.dd { dd2 = "111", dd_Column = "222" });
            var data2 = con.Execute<DataModel.dd>("SELECT * FROM dd");

            var query = con.dd.Where(c => c.dd2 == "eee2");
            var data = query.ToList();

            var query2 = con.dd.Where(c => c.dd2 == "eee2").Set(c => c.dd2, "333");
            query2.Update();

            con.Commit();
        }


        dynamic args = new System.Dynamic.ExpandoObject();
        args.token = token;
        args.arg = arg.Out;
        if (arg.Out.B == "ex")
        {
            throw new System.Exception("Method exception!");
        }

        var exit = await RedisHelper.HExistsAsync("Role", "value2");

        if (exit)
        {
            arg.Out.B = await RedisHelper.HGetAsync("Role", "value2");
        }

        return this.ResultCreate(args);
    }

    public virtual async Task Test002(Business.Auth.Token token, Arg<Test001> arg, [Ignore(IgnoreMode.BusinessArg)][Test2]decimal mm = 0.0234m)
    {
        dynamic args = new System.Dynamic.ExpandoObject();
        args.token = token;
        args.arg = arg.Out;
        if (arg.Out.B == "ex")
        {
            throw new System.Exception("Method exception!");
        }
    }

    public virtual async Task Test003(Business.Auth.Token token, Arg<Test001> arg, [Ignore(IgnoreMode.BusinessArg)][Test2]decimal mm = 0.0234m)
    {
        dynamic args = new System.Dynamic.ExpandoObject();
        args.token = token;
        args.arg = arg.Out;
        if (arg.Out.B == "ex")
        {
            throw new System.Exception("Method exception!");
        }
    }
}

public class TestAttribute : ArgumentAttribute
{
    public TestAttribute(int state = 111, string message = null) : base(state, message) { }

    public override async ValueTask<IResult> Proces(dynamic value)
    {
        using (var con = Context.DataBase.DB.GetConnection())
        {
            con.Save(new DataModel.dd { dd2 = "111", dd_Column = "222" });
        }

        var exit = await RedisHelper.HExistsAsync("Role", "value2");

        switch (value)
        {
            case "ok":
                return this.ResultCreate(exit ? await RedisHelper.HGetAsync("Role", "value2") : "not!");

            case "error":
                return this.ResultCreate(this.State, $"{this.Nick} cannot be empty");

            case "data":
                return this.ResultCreate(value + "1122");

            default: throw new System.Exception("Argument exception!");
        }
    }
}

public class Test2Attribute : ArgumentAttribute
{
    public Test2Attribute(int state = 112, string message = null) : base(state, message) { }

    public override async ValueTask<IResult> Proces(dynamic value)
    {
        return this.ResultCreate(value + 0.1m);
    }
}

public class Args
{
    public class Test001
    {
        [Test]
        [Nick("password")]
        public string A { get; set; }

        public string B { get; set; }
    }
}