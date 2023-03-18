#define DotNetPrettyName "Microsoft .NET Desktop Runtime"
#define DotNetName "Microsoft.WindowsDesktop.App 6"
#define DotNetVersion "6.0.15"
#define DotNetURL "https://download.visualstudio.microsoft.com/download/pr/513d13b7-b456-45af-828b-b7b7981ff462/edf44a743b78f8b54a2cec97ce888346/windowsdesktop-runtime-6.0.15-win-x64.exe"
#define DotNetExeName "dotnet6.exe"
#define DotNetExeArgs "/install /repair /passive /norestart"

[Code]
var
  NeedsInstall: Boolean;
  Dependency_DownloadPage: TDownloadWizardPage;
  Dependency_Memo: String;

function SplitString(Text: String; Separator: String): TArrayOfString;
var
  i, p: Integer;
  dest: TArrayOfString; 
begin
  i := 0;
  repeat
    SetArrayLength(dest, i + 1);
    p := Pos(Separator, Text);
    if p > 0 then begin
      dest[i] := Copy(Text, 1, p - 1);
      Text := Copy(Text, p + Length(Separator), Length(Text));
      i := i + 1;         
    end else begin
      dest[i] := Text;
      Text := '';
    end;
  until Length(Text)=0;
  Result := dest
end;

function CheckDotNetVersionEqualOrHigher(StringArray: TArrayOfString; DotNetName: String; MinimumVersionString: String): Boolean;
var
  i, p: Integer;
  str, verStr: String;
  strArray: TArrayOfString;
  minVer, ver: Int64;
begin       
  Result := False;
  if StrToVersion(MinimumVersionString, minVer) then begin
    for i := 0 to GetArrayLength(StringArray) - 1 do begin
      str := StringArray[i];
      p := Pos(DotNetName, str);
      if p > 0 then begin
        strArray := SplitString(str, ' ');
        if GetArrayLength(strArray) >= 3 then begin
          verStr := strArray[1]; 
          if StrToVersion(verStr, ver) then begin
            if ComparePackedVersion(minVer, ver) <= 0 then begin
              Result := True;
              break;
            end;
          end;
        end;
      end;
    end;
  end;
end;

function IsDotNetInstalled(DotNetName: string; DotNetVersion: string): Boolean;
var
  cmd, args, fileName, command: string;
  output: AnsiString;
  resultCode: Integer;
begin  
  command := 'dotnet --list-runtimes';
  fileName := ExpandConstant('{tmp}\dotnet.txt');
  cmd := ExpandConstant('{cmd}');
  args := '/C ' + command + ' > "' + fileName + '" 2>&1';

  if Exec(cmd, args, '', SW_HIDE, ewWaitUntilTerminated, resultCode) and (resultCode = 0) then begin
    if LoadStringFromFile(fileName, output) then begin
      if CheckDotNetVersionEqualOrHigher(SplitString(output, #13#10), DotNetName, DotNetVersion) then begin
        Log('"' + DotNetName + '" version "' + DotNetVersion + '" or higher found in output of "' + command + '"');
        Result := True;
      end
      else begin
        Log('"' + DotNetName + '" version "' + DotNetVersion + '" or higher not found in output of "' + command + '"');
        Result := False;
      end;
    end
    else begin
      Log('Failed to read output of "' + command + '"');
      Result := False;
    end;
  end
  else begin
    Log('Failed to execute "' + command + '"');
    Result := False;
  end;
 
  DeleteFile(fileName);
end;

procedure InstallDotNet6DesktopRuntime;
begin
  if not IsDotNetInstalled('{#DotNetName}', '{#DotNetVersion}') then begin 
    NeedsInstall := True;
    Dependency_Memo := Dependency_Memo + #13#10 + '%1' + '{#DotNetPrettyName} {#DotNetVersion}';
  end;
end;

<event('InitializeWizard')>
procedure Dependency_InitializeWizard;
begin
  Dependency_DownloadPage := CreateDownloadPage(SetupMessage(msgWizardPreparing), SetupMessage(msgPreparingDesc), nil);
end;

<event('UpdateReadyMemo')>
function Dependency_UpdateReadyMemo(const Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
begin
  Result := '';
  if MemoUserInfoInfo <> '' then begin
    Result := Result + MemoUserInfoInfo + Newline + NewLine;
  end;
  if MemoDirInfo <> '' then begin
    Result := Result + MemoDirInfo + Newline + NewLine;
  end;
  if MemoTypeInfo <> '' then begin
    Result := Result + MemoTypeInfo + Newline + NewLine;
  end;
  if MemoComponentsInfo <> '' then begin
    Result := Result + MemoComponentsInfo + Newline + NewLine;
  end;
  if MemoGroupInfo <> '' then begin
    Result := Result + MemoGroupInfo + Newline + NewLine;
  end;
  if MemoTasksInfo <> '' then begin
    Result := Result + MemoTasksInfo;
  end;
  if Dependency_Memo <> '' then begin
    if MemoTasksInfo = '' then begin
      Result := Result + SetupMessage(msgReadyMemoTasks);
    end;
    Result := Result + FmtMessage(Dependency_Memo, [Space]);
  end;
end;

<event('PrepareToInstall')>
function Dependency_PrepareToInstall(var NeedsRestart: Boolean): String;
var
  prettyName: String;
  retry, abort: Boolean;
  resultCode: Integer;
begin
  if NeedsInstall then begin
    prettyName := '{#DotNetPrettyName} {#DotNetVersion}'

    Dependency_DownloadPage.Show;
    Dependency_DownloadPage.Clear;
    Dependency_DownloadPage.Add('{#DotNetURL}', '{#DotNetExeName}', '');

    retry := True;
    while retry do begin
      retry := False;
      abort := False;

      try
        Dependency_DownloadPage.Download;
      except
        if Dependency_DownloadPage.AbortedByUser then begin
          abort := True
          end else begin
          case SuppressibleMsgBox(AddPeriod(GetExceptionMessage), mbError, MB_RETRYCANCEL, IDRETRY) of
            IDCANCEL: begin
              abort := True;
            end;
            IDRETRY: begin
              retry := True;
            end;
          end;
        end;
      end;
    end;

    if not abort then begin
      Dependency_DownloadPage.SetText(prettyName, '');
      Dependency_DownloadPage.SetProgress(1, 1);

      while True do begin
          resultCode := 0;
          if ShellExec('', ExpandConstant('{tmp}{\}') + '{#DotNetExeName}', '{#DotNetExeArgs}', '', SW_SHOWNORMAL, ewWaitUntilTerminated, resultCode) then begin
            if (resultCode = 0) then begin
              break;
            end;
          end;

          case SuppressibleMsgBox(FmtMessage(SetupMessage(msgErrorFunctionFailed), [prettyName, IntToStr(ResultCode)]), mbError, MB_RETRYCANCEL, IDRETRY) of
            IDCANCEL: begin
              abort := True;
              break;
            end;
          end;
        end;
    end;

    Dependency_DownloadPage.Hide;
  end;
end;
