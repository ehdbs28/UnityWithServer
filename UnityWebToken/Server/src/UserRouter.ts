import { Router, Request, Response } from "express";
import { Pool } from "./DB";
import { ResultSetHeader, RowDataPacket } from "mysql2/promise";
import { MessageType, ResponseMSG } from "./Types";

export const userRouter :Router = Router();

userRouter.get("/user/login", async (req :Request, res :Response) => {
    // 여긴 구현 안할거임
});

userRouter.post("/user/login", async (req :Request, res :Response) => {
    let { email, password } :{ email :String, password :String } = req.body;

    console.log(email, password);
});

userRouter.get("/user/register", async (req :Request, res :Response) => {
    res.render("register");
});

userRouter.post("/user/register", async (req :Request, res :Response) => {
    let email :string = req.body.email;
    let password :string = req.body.password;
    let passwordc :string = req.body.passwordc;
    let name :string = req.body.username;

    if(email == "" || password == "" || name == ""){
        let msg :ResponseMSG = { type: MessageType.ERROR, message: "필수값이 비어있습니다." };
        res.json(msg);
        return;
    }

    if(password != passwordc){
        let msg :ResponseMSG = { type: MessageType.ERROR, message: "비밀번호와 확인이 일치하지 않습니다." };
        res.json(msg);
        return;
    }

    const sql :string = "INSERT INTO users(email, password, name) VALUES(?, PASSWORD(?), ?)";
    let [result, info] :[ResultSetHeader, any] = await Pool.execute(sql, [email, password, name]);

    if(result.affectedRows != 1){
        let msg :ResponseMSG = { type: MessageType.ERROR, message: "DB에 이상이 있습니다. 관리자에게 문의해주세요." };
        res.json(msg);
        return;
    }

    let msg :ResponseMSG = { type: MessageType.SUCCESS, message: "성공적으로 회원가입 되었습니다." };
    res.json(msg);
});
