[Code]
const EnvironmentKey = 'Environment';

procedure EnvAddPath(Path: string);
var
    Paths: string;
begin
    if not RegQueryStringValue(HKEY_CURRENT_USER, EnvironmentKey, 'Path', Paths) then begin
        Paths := '';
    end;
    if Pos(';' + Uppercase(Path) + ';', ';' + Uppercase(Paths) + ';') > 0 then exit;
    
    Paths := Paths + ';'+ Path +';'
    if RegWriteStringValue(HKEY_CURRENT_USER, EnvironmentKey, 'Path', Paths) then begin
        Log(Format('The [%s] added to PATH: [%s]', [Path, Paths]))
    end else begin
        Log(Format('Error while adding the [%s] to PATH: [%s]', [Path, Paths]));
    end;
end;

procedure EnvRemovePath(Path: string);
var
    Paths: string;
    P: Integer;
begin
    if not RegQueryStringValue(HKEY_CURRENT_USER, EnvironmentKey, 'Path', Paths) then exit;

    P := Pos(';' + Uppercase(Path) + ';', ';' + Uppercase(Paths) + ';');
    if P = 0 then exit;
    Delete(Paths, P - 1, Length(Path) + 1);
    if RegWriteStringValue(HKEY_CURRENT_USER, EnvironmentKey, 'Path', Paths) then begin
        Log(Format('The [%s] removed from PATH: [%s]', [Path, Paths]))
    end else begin
        Log(Format('Error while removing the [%s] from PATH: [%s]', [Path, Paths]));
    end;
end;
