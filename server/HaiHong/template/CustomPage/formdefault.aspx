
{=Config(sitetitle)}

{TS.SQL SQL="select top 10 * from ts_article"}
<a href="/show/1/{$classId}/{$id}.aspx">{$title}</a><br/>
{/TS.SQL}