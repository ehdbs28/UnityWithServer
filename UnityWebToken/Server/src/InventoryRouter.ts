import { Router, Request, Response } from "express";
import { Pool } from "./DB";
import { ResultSetHeader, RowDataPacket } from "mysql2/promise";
import { InventoryVO, MessageType, ResponseMSG, UserVO } from "./Types";

export const InventoryRouter = Router();

InventoryRouter.get("/inven", async (req :Request, res :Response) => {
    if(req.user == null){
        res.json({ type: MessageType.ERROR, message: "권한이 없습니다." });
        return;
    }

    const user = req.user;

    let sql = "SELECT json FROM Inventories WHERE user_id = ?";
    let [rows, col] : [RowDataPacket[], any] = await Pool.query(sql, [user.id]);

    if(rows.length == 0){
        res.json({ type: MessageType.EMPTY, message: "" });
    }
    else{
        let json = rows[0].json as string;
        res.json({ type: MessageType.SUCCESS, message: json });
    }
});

InventoryRouter.post("/inven", async (req :Request, res :Response) => {
    if(req.user == null){
        res.json({ type: MessageType.ERROR, message: "권한이 없습니다." });
        return;
    }

    const user = req.user;
    let json = req.body.json;
    let vo :InventoryVO = JSON.parse(json);

    // 인벤토리에 현재 userid로 무엇이 있는지
    // 없으면 처음 저장하는 상태임
    let [rows, cols] : [RowDataPacket[], any] = await Pool.query("SELECT * FROM newInventories WHERE user_id = ?", [user.id]);

    if(rows.length == 0){
        let insertSQL = `INSERT INTO newInventories(user_id, slot_number, item_code, count) VALUES `;
        let bindDataArr = [];
        for(let i = 0; i < vo.count; i++){
            insertSQL += i == vo.count - 1 ? `(?, ?, 0, 0)` : `(?, ?, 0, 0), `;
            bindDataArr.push(user.id);
            bindDataArr.push(i);
        }

        let [result, info] : [ResultSetHeader, any] = await Pool.execute(insertSQL, bindDataArr);
    }
    else{
        let updateSQL = "UPDATE newInventories SET item_code = 0, count = 0 WHERE user_id = ?";
        await Pool.execute(updateSQL, [user.id]);
    }

    // 인벤토리 업데이트
    let updateSQL = "UPDATE newInventories SET item_code = ?, count = ? WHERE slot_number = ? AND user_id = ?";
    for(let i = 0; i < vo.list.length; i++){
        const item = vo.list[i];
        await Pool.execute(updateSQL, [item.itemCode, item.count, item.slotNumber, user.id]);
    }

    const sql = "INSERT INTO Inventories (user_id, json) VALUES (?, ?) ON DUPLICATE KEY UPDATE json = VALUES(json)";
    let [result, info] :[ResultSetHeader, any] = await Pool.execute(sql, [user.id, JSON.stringify(vo)]);
    // result.affectedRows 갑이 2면 기존에 존재해서 업데이트, 1이면 없어서 insert

    let msg: ResponseMSG = { type: MessageType.SUCCESS, message: "인벤토리 저장완료" };
    res.json(msg);
});