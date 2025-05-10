export interface User {
    id: string;
    display_name: string;
    validade?: any;
    tipoUso: number;
    quantidadeUso:number;
    status?: boolean;
    seriais?: string[];
    identificador?:string,
    log?: boolean;
  }
  