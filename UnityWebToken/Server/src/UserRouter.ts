import { Router, Request, Response } from "express";
import { Pool } from "./DB";
import { ResultSetHeader, RowDataPacket } from "mysql2/promise";
import { MessageType, ResponseMSG, UserVO } from "./Types";
import { createJWT } from "./MyJWT";

export const userRouter :Router = Router();

userRouter.get("/user", async (req :Request, res :Response) => {
    let resMsg : ResponseMSG = { type: MessageType.EMPTY, message: "로그인이 필요합니다." };

    if(req.user != null){
        // console.log(req.user);
        resMsg = {type: MessageType.SUCCESS, message: JSON.stringify(req.user) };
        res.json(resMsg);
    }
    else{
        res.json(resMsg);
    }
});

userRouter.get("/user/login", async (req :Request, res :Response) => {
    // 여긴 구현 안할거임
});

userRouter.post("/user/login", async (req :Request, res :Response) => {
    let { email: inputEmail, password } :{ email :String, password :String } = req.body;

    const sql = "SELECT * FROM users WHERE email = ? AND password = PASSWORD(?)";
    let [row, col] : [RowDataPacket[], any] = await Pool.query(sql, [inputEmail, password]);

    if(row.length == 0){
        res.json({ type: MessageType.ERROR, message: "아이디 또는 비밀번호가 올바르지 않습니다." });
        return;
    }

    let { id, email, name, exp } = row[0];

    let user :UserVO = { id, email, name, exp };
    let token = createJWT(user);
    res.json({ type: MessageType.SUCCESS, message: JSON.stringify({token, user}) });
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
