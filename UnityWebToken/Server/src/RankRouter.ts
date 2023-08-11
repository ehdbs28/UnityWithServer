import { Router, Request, Response } from "express";
import { Pool } from "./DB";
import { ResultSetHeader, RowDataPacket } from "mysql2/promise";
import { MessageType, ResponseMSG, UserVO, rankVO } from "./Types";

export const RankRouter = Router();

RankRouter.get("/rank", async (req: Request, res: Response) => {
    if(req.user == null){
        res.json({ type: MessageType.ERROR, message: "권한이 없습니다." });
        return;
    }

    const user = req.user;

    let sql = "SELECT * FROM ranking ORDER BY score DESC LIMIT 0,3";
    let [rows, cols] : [RowDataPacket[], any] = await Pool.query(sql);

    let json = {list:rows};
    res.json({ type: MessageType.SUCCESS, message: JSON.stringify(json) });
});

RankRouter.post("/rank", async (req: Request, res: Response) => {
    if(req.user == null){
        res.json({ type: MessageType.ERROR, message: "권한이 없습니다." });
        return;
    }

    const user = req.user;
    let json = req.body;
    
    let {user_name, score, memo} = json;

    const sql = "INSERT INTO ranking(user_id, user_name, score, memo) VALUES (?, ?, ?, ?)";
    let [result, info] :[ResultSetHeader, any] = await Pool.execute(sql, [user.id, user_name, score, memo]);

    let msg: ResponseMSG = { type: MessageType.SUCCESS, message: "랭킹 갱신완료" };
    res.json(msg);
});