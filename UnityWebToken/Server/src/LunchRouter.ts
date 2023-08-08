import { Router, Request, Response } from "express";
import axios from "axios";
import {load, CheerioAPI} from "cheerio";
import iconv from "iconv-lite";
import { Pool } from "./DB";
import { RowDataPacket } from "mysql2/promise";

// 익스프레스 라우터 하나 생성
export const lunchRouter :Router = Router();

lunchRouter.get("/lunch", async (req :Request, res :Response) => {
    let date :string = req.query.date as string;
    // const date :string = "20230703";

    // DB에 데이터가 있는지 확인
    let result = await GetDataFromDB(date);
    
    if(result != null){
        // DB 정보를 보내주면 된다.
        let json = { date, menus: JSON.parse(result[0].menu) };
        res.render("lunch", json);
        return;
    }

    const url :string = `https://ggm.hs.kr/lunch.view?date=${date}`;

    // 비동기 함수 Async Task
    let html = await axios({url, method: "GET", responseType: "arraybuffer"});

    // 데이터 통신은 "모든 데이터"를 바이트 스트림으로 통신
    let data :Buffer = html.data;
    let decoded = iconv.decode(data, "euc-kr");

    // HTML 문자열을 Cheerio에서 로드해서 Cheerio객체로 만듬
    const $ :CheerioAPI = load(decoded);

    let text:string = $(".menuName > span").text();
    let menus :string[] = text.split("\n").map(x => x.replace(/[0-9]+\./g, "")).filter(x => x.length > 0);

    const json = { date, menus };

    // res.json({id: 1, text: menus});
    // ejs, pug, nunjucks
    res.render("lunch", json);

    await Pool.execute("INSERT INTO lunch(date, menu) VALUES(?, ?)", [date, JSON.stringify(menus)]);
});

async function GetDataFromDB(date :string){
    // Sql injection 방지
    const sql :string = "SELECT * FROM lunch WHERE date = ?";
    let [row, col] = await Pool.query(sql, [date]);
    row = row as RowDataPacket[];

    return row.length > 0 ? row : null;
}