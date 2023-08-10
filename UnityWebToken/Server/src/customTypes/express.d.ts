import { UserVO } from "../Types";

declare global{
    namespace Express{
        interface Request{
            user: UserVO | null
        }
    }
}